using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace FunWithPolygons
{
    class NarrowPhase
    {
        Game game;
        public Vector2 impulse;
        public Vector2 rV;
        public float vN;
        public float eMin;

        public float Cross(Vector2 a, Vector2 b)
        {
            return (a.X * b.Y) - (a.Y * b.X);
        }

        public Vector2 Cross(Vector2 v, float a)
        {
            return new Vector2(a * v.Y, -a * v.X);
        }

        public Vector2 Cross(float a, Vector2 v)
        {
            return new Vector2(-a * v.Y, a * v.X);
        }

        public NarrowPhase(Game g)
        {
            game = g;
        }

        /// <summary>
        /// Resolves the collision of 2 objects using the impulse by calculating the potential change in the object's momentum
        /// </summary>
        /// <param name="pair"></param>
        public void ResolveCollision(Pair pair)
        {
            //  V(AB) = V(B) - V(A)
            //  n = normal vector
            //  V'(AB) = -V(AB) dot n = -((V(B) - V(A)) dot n)
            //  j = impulse   (scalar value)
            //  V' = V + j * n
            //  j = mass*velocity
            //  velocity = j/mass
            //  V' = V + (j*n)/mass -> V'(A) = V(A) - (j*n)/mass(A)   &   V(B) = V(B) + (j*n)/mass(B)
            //  V'(AB) = (V'(A) - V'(B)) * n = -((V(B) - V(A)) dot n)
            //  {[V(B) + (j*n)/mass(A) dot n) + (V(A) - (j*n)/mass(B)]} dot n + (V(B) - V(A)) dot n = 0
            //  [V(B) - V(A)] dot n + j*n * (1/mass(A) + 1/mass(B)) dot n + (V(B) - V(A)) dot n = 0
            //  -j*(1/mass(A) + 1/mass(B)) * n dot n = (V(B) - V(A)) dot n + (V(B) - V(A)) dot n = 2 * [(V(B) - V(A)) dot n] = 2 * (V(AB) dot n)
            //  n dot n = 1
            //  j = -2 * (V(AB) dot n / (1/mass(A) + 1/mass(B)

            float kF = (pair.A.DynamicFriction * pair.A.DynamicFriction);
            float sF = (pair.A.StaticFriction * pair.A.StaticFriction);

            rV = pair.B.Velocity - pair.A.Velocity;
            Vector2 contactVector = pair.B.Position - pair.A.Position;
            vN = Vector2.Dot(rV, Pair.Normal);

            if (vN > 0)
                return;

            eMin = Math.Min(pair.A.Restitution, pair.B.Restitution);

            float j = -(1 + eMin) * vN;
            j /= pair.A.InvMass + pair.B.InvMass;

            impulse = j * Pair.Normal;
            pair.A.ApplyImpulse(-impulse, contactVector);
            pair.B.ApplyImpulse(impulse, contactVector);

            //Same as previous equation but replacing rV with t, j with jt and impulse with tangentImpulse

            Vector2 t = rV - (Pair.Normal * Vector2.Dot(rV, Pair.Normal));
            if (t != Vector2.Zero)
                t.Normalize();

            float jt = -Vector2.Dot(rV, t);
            jt /= pair.A.InvMass + pair.B.InvMass;

            if (jt == 0.0f)
                return;

            //Checks to see if jt is less than the static friction, if so applys static friction, if not applies kinetic friction
            Vector2 tangentImpulse;
            if (Math.Abs(jt) < j * sF)
                tangentImpulse = t * jt;
            else
                tangentImpulse = t * -j * kF;

            pair.A.ApplyImpulse(-tangentImpulse, t);
            pair.B.ApplyImpulse(tangentImpulse, t);
        }

        /// <summary>
        /// Corrects the position of the objects by a percentage of the penetration
        /// while ignoring a certain "slop" amount of penetration
        /// The goal is to prevent the object from sinking into each other
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="p"></param>
        /// <param name="s"></param>
        public void PositionalCorrection(Pair pair, float p, float s)
        {
            float percent = p;
            float slop = s;
            Vector2 correction = (Math.Max(Pair.Penetration - slop, 0.0f) / (pair.A.InvMass + pair.B.InvMass)) * percent * Pair.Normal;
            pair.A.Position -= pair.A.InvMass * correction;
            pair.B.Position += pair.B.InvMass * correction;
        }

        /// <summary>
        /// Finds the axis of least penetration by finding the greatest support point in the direction opposite the face normal for each face of the polygon
        /// </summary>
        /// <param name="faceIndex">Saves the index of the face with the greatest support point, which is also the face that is being collided with</param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public float FindAxisofLeastPenetration(ref int faceIndex, PerfectPolygon A, PerfectPolygon B)
        {
            float bestDistance = -float.MaxValue;
            int bestIndex = 0;

            for (int i = 0; i < A.points.Length; i++)
            {
                Vector2 n = A.faceNormals[i];
                //Return vertex farthest in the -n direction
                Vector2 s = B.GetSupport(-n);
                Vector2 p = A.points[i];

                //Calculates the distance along the face normal of the vector from p to s
                float d = Vector2.Dot(n, s - p);

                if (d > bestDistance)
                {
                    bestDistance = d;
                    bestIndex = i;
                }
            }

            faceIndex = bestIndex;
            return bestDistance;
        }

        /// <summary>
        /// Finds the incident face on the inciting polygon
        /// </summary>
        /// <param name="p">Saves the 2 points that make up the incident face</param>
        /// <param name="reference"></param>
        /// <param name="incident"></param>
        /// <param name="referenceIndex">Index of the inciting face</param>
        public void FindIncidentFace(ref Vector2[] p, PerfectPolygon reference, PerfectPolygon incident, int referenceIndex)
        {
            //Face of the reference polygon that is being collided with
            Vector2 referenceNormal = reference.faceNormals[referenceIndex];

            int incidentFace = 0;
            float minD = float.MaxValue;

            //Loops through all of the faces of the inciting polygon and determines the face most perpendicular
            //to the face of the reference polygon that is being collided with
            for (int i = 0; i < incident.points.Length; i++)
            {
                float d = Vector2.Dot(referenceNormal, incident.faceNormals[i]);
                if (d < minD)
                {
                    minD = d;
                    incidentFace = i;
                }
            }

            //Saves the faces of the 2 points that make up the inciting face on the inciting polygon
            p[0] = incident.points[incidentFace];
            p[1] = incident.points[incidentFace + 1 >= (int)incident.points.Length ? 0 : incidentFace + 1];
        }

        /// <summary>
        /// Determines the inciting and reference polygon by comparing their penetration values
        /// </summary>
        /// <param name="aPenetration"></param>
        /// <param name="bPenetration"></param>
        /// <returns></returns>
        public bool BiasGreaterThan(float aPenetration, float bPenetration)
        {
            const float biasRelative = 0.95f;
            const float biasAbsolute = 0.01f;
            return aPenetration >= bPenetration * biasRelative + aPenetration * biasAbsolute;
        }

        /// <summary>
        /// Tests to see if 2 polygons are colliding
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool PolygonvsPolygon(Pair pair)
        {
            PerfectPolygon A = (pair.A.Shape as PerfectPolygon);
            PerfectPolygon B = (pair.B.Shape as PerfectPolygon);
            Pair.ContactCount = 0;

            //find the index of the reference face and return the distance of the greatest support point
            int faceA = 0;
            float penetrationA = FindAxisofLeastPenetration(ref faceA, A, B);
            //if the support point is positive it means the objects have not collided
            if (penetrationA >= 0.0f)
                return false;

            int faceB = 0;
            float penetrationB = FindAxisofLeastPenetration(ref faceB, B, A);
            if (penetrationB >= 0.0f)
                return false;

            int referenceIndex = 0;
            bool flip;

            PerfectPolygon refPoly;
            PerfectPolygon incPoly;

            //check to see which of the penetrations is greater and assign the reference and incident polygons as such
            if (BiasGreaterThan(penetrationA, penetrationB))
            {
                refPoly = A;
                incPoly = B;
                referenceIndex = faceA;
                flip = false;
            }

            else
            {
                refPoly = B;
                incPoly = A;
                referenceIndex = faceB;
                flip = true;
            }

            //Find the inciting face of the inciting polygon
            Vector2[] incidentFacePoints = new Vector2[2];
            FindIncidentFace(ref incidentFacePoints, refPoly, incPoly, referenceIndex);

            //points that form the inciting face
            Vector2 point1 = refPoly.points[referenceIndex];
            Vector2 point2 = refPoly.points[referenceIndex + 1 == refPoly.points.Length ? 0 : referenceIndex + 1];

            Vector2 refFaceNormal = refPoly.faceNormals[referenceIndex];

            //Distance from the origion along the reference face normal
            float refC = Vector2.Dot(refFaceNormal, point1);

            //Calculations were made from the incient to the reference but the method is running to find A to B
            //So if A is the inciting object then the normal is backwards and needs to be flipped
            Pair.Normal = flip ? -refFaceNormal : refFaceNormal;

            int cp = 0;

            //Distance from origin of the first incident face point along reference face normal minus the one for reference face
            float separation = Vector2.Dot(refFaceNormal, incidentFacePoints[0]) - refC;
            if (separation <= 0.0f)
            {
                //add the currenct seperation to the penetration
                Pair.Contacts[cp] = incidentFacePoints[0];
                Pair.Penetration += -separation;
                Pair.ContactCount++;
                cp++;
            }

            else
                Pair.Penetration = 0;

            //same as previous but for the other incident face point
            separation = Vector2.Dot(refFaceNormal, incidentFacePoints[1]) - refC;
            if (separation <= 0.0f)
            {
                Pair.Contacts[cp] = incidentFacePoints[1];
                Pair.Penetration += -separation;
                Pair.ContactCount++;
                cp++;

                //average the penetration
                Pair.Penetration /= (float)cp;
            }

            return true;
        }

        public bool CheckCollision(Pair pair)
        {
            return PolygonvsPolygon(pair);
        }
    }
}
