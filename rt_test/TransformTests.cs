using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;

namespace rt_test
{
    [TestClass]
    public class TransformTests
    {
        [TestMethod]
        public void TransformIsAsExpected()
        {
            var transform = Transform.Translation(5, -3, 2);
            Assert.AreEqual(new Matrix(new List<double>{
                1, 0, 0, 5,
                0, 1, 0, -3,
                0, 0, 1, 2,
                0, 0, 0, 1

                }), transform);
        }

        [TestMethod]
        public void MultiplyByTranslationMatrix()
        {
            var transform = Transform.Translation(5, -3, 2);
            var point = new Point(-3, 4, 5);
            Assert.AreEqual(new Point(2, 1, 7), transform * point);
        }


        [TestMethod]
        public void MultiplyByTheInverseOfTranslationMatrix()
        {
            var transform = Transform.Translation(5, -3, 2);
            var inv = transform.Inverse();
            var point = new Point(-3, 4, 5);
            Assert.AreEqual(new Point(-8, 7, 3), inv * point);
        }

        [TestMethod]
        public void TranslationDoesNotAffectVectors()
        {
            var transform = Transform.Translation(5, -3, 2);
            var vector = new Vector(-3, 4, 5);
            Assert.AreEqual(vector, transform * vector);
        }

        [TestMethod]
        public void ScalingMatrixAppliedToPoint()
        {
            var transform = Transform.Scaling(2, 3, 4);
            var point = new Point(-4, 6, 8);
            Assert.AreEqual(new Point(-8, 18, 32), transform * point);
        }

        [TestMethod]
        public void ScalingMatrixAppliedToVector()
        {
            var transform = Transform.Scaling(2, 3, 4);
            var vector = new Vector(-4, 6, 8);
            Assert.AreEqual(new Vector(-8, 18, 32), transform * vector);
        }

        [TestMethod]
        public void InverseScalingMatrixAppliedToVector()
        {
            var transform = Transform.Scaling(2, 3, 4).Inverse();
            var vector = new Vector(-4, 6, 8);
            Assert.AreEqual(new Vector(-2, 2, 2), transform * vector);
        }

        [TestMethod]
        public void ReflectionIsScalingByNegativeValue()
        {
            var transform = Transform.Scaling(-1, 1, 1);
            var point = new Point(2, 3, 4);
            Assert.AreEqual(new Point(-2, 3, 4), transform * point);
        }

        [TestMethod]
        public void RotationAroundXAxis()
        {
            var point = new Point(0, 1, 0);
            var half_quarter = Transform.RotationX(Math.PI / 4);
            var full_quarter = Transform.RotationX(Math.PI / 2);

            Assert.AreEqual(new Point(0, Math.Sqrt(2) / 2, Math.Sqrt(2) / 2), half_quarter * point);
            Assert.AreEqual(new Point(0, 0, 1), full_quarter * point);
        }

        [TestMethod]
        public void InverseRotationAroundXAxisIsOppositeDirection()
        {
            var point = new Point(0, 1, 0);
            var half_quarter = Transform.RotationX(Math.PI / 4).Inverse();

            Assert.AreEqual(new Point(0, Math.Sqrt(2) / 2, -Math.Sqrt(2) / 2), half_quarter * point);
        }

        [TestMethod]
        public void RotationAroundYAxis()
        {
            var point = new Point(0, 0, 1);
            var half_quarter = Transform.RotationY(Math.PI / 4);
            var full_quarter = Transform.RotationY(Math.PI / 2);

            Assert.AreEqual(new Point(Math.Sqrt(2) / 2, 0, Math.Sqrt(2) / 2), half_quarter * point);
            Assert.AreEqual(new Point(1, 0, 0), full_quarter * point);
        }

        [TestMethod]
        public void RotationAroundZAxis()
        {
            var point = new Point(0, 1, 0);
            var half_quarter = Transform.RotationZ(Math.PI / 4);
            var full_quarter = Transform.RotationZ(Math.PI / 2);

            Assert.AreEqual(new Point(-Math.Sqrt(2) / 2, Math.Sqrt(2) / 2, 0), half_quarter * point);
            Assert.AreEqual(new Point(-1, 0, 0), full_quarter * point);
        }

        [TestMethod]
        public void ShearingMovesXInProportionToY()
        {
            var transform = Transform.Shearing(1, 0, 0, 0, 0, 0);
            var point = new Point(2, 3, 4);
            Assert.AreEqual(new Point(5, 3, 4), transform * point);
        }

        [TestMethod]
        public void ShearingMovesXInProportionToZ()
        {
            var transform = Transform.Shearing(0, 1, 0, 0, 0, 0);
            var point = new Point(2, 3, 4);
            Assert.AreEqual(new Point(6, 3, 4), transform * point);
        }

        [TestMethod]
        public void ShearingMovesYInProportionToX()
        {
            var transform = Transform.Shearing(0, 0, 1, 0, 0, 0);
            var point = new Point(2, 3, 4);
            Assert.AreEqual(new Point(2, 5, 4), transform * point);
        }

        [TestMethod]
        public void ShearingMovesYInProportionToZ()
        {
            var transform = Transform.Shearing(0, 0, 0, 1, 0, 0);
            var point = new Point(2, 3, 4);
            Assert.AreEqual(new Point(2, 7, 4), transform * point);
        }

        [TestMethod]
        public void ShearingMovesZInProportionToX()
        {
            var transform = Transform.Shearing(0, 0, 0, 0, 1, 0);
            var point = new Point(2, 3, 4);
            Assert.AreEqual(new Point(2, 3, 6), transform * point);
        }

        [TestMethod]
        public void ShearingMovesZInProportionToY()
        {
            var transform = Transform.Shearing(0, 0, 0, 0, 0, 1);
            var point = new Point(2, 3, 4);
            Assert.AreEqual(new Point(2, 3, 7), transform * point);
        }

        [TestMethod]
        public void IndividualTransformationsAreAppliedInSequence()
        {
            var p = new Point(1, 0, 1);
            var A = Transform.RotationX(Math.PI / 2);
            var B = Transform.Scaling(5, 5, 5);
            var C = Transform.Translation(10, 5, 7);
            var p2 = A * p;
            Assert.AreEqual(new Point(1, -1, 0), p2);
            var p3 = B * p2;
            Assert.AreEqual(new Point(5, -5, 0), p3);
            var p4 = C * p3;
            Assert.AreEqual(new Point(15, 0, 7), p4);
        }

        [TestMethod]
        public void ChainedTransformationsMustBeAppliedInReverseOrder()
        {
            var p = new Point(1, 0, 1);
            var A = Transform.RotationX(Math.PI / 2);
            var B = Transform.Scaling(5, 5, 5);
            var C = Transform.Translation(10, 5, 7);
            var T = C * B * A;
            Assert.AreEqual(new Point(15, 0, 7), T * p);
        }

        [TestMethod]
        public void TransformationMatrixForTheDefaultOrientation()
        {
            var from = new Point(0, 0, 0);
            var to = new Point(0, 0, -1);
            var up = new Vector(0, 1, 0);
            var t = Transform.View(from, to, up);
            Assert.AreEqual(Matrix.Identity(4), t);
        }

        [TestMethod]
        public void AViewTransformationMatrixLookingInPositiveZDirection()
        {
            var from = new Point(0, 0, 0);
            var to = new Point(0, 0, 1);
            var up = new Vector(0, 1, 0);
            var t = Transform.View(from, to, up);
            Assert.AreEqual(Transform.Scaling(-1, 1, -1), t);
        }

        [TestMethod]
        public void TheViewTransformMoveTheWorld()
        {
            var from = new Point(0, 0, 8);
            var to = new Point(0, 0, 0);
            var up = new Vector(0, 1, 0);
            var t = Transform.View(from, to, up);
            Assert.AreEqual(Transform.Translation(0, 0, -8), t);
        }

        [TestMethod]
        public void AnArbitraryViewTransformation()
        {
            var from = new Point(1, 3, 2);
            var to = new Point(4, -2, 8);
            var up = new Vector(1, 1, 0);
            var t = Transform.View(from, to, up);
            var m = new Matrix(new List<double>
            {
                -0.50709f, 0.50709f, 0,67612f, -2.36643f,
                0.76772f, 0.60609f, 0.12122f, -2.82843f,
                -0.35857f, 0.59761f, -0.71714f, 0.0000f,
                0.00000f, 0.00000f, 0.00000f, 1.00000f
            });
        }
    }
}
