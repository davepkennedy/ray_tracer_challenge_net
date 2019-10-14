using Microsoft.VisualStudio.TestTools.UnitTesting;

using rt;
using static rt.Constants;

using System.Collections.Generic;

namespace rt_test
{
    [TestClass]
    public class MatrixTests
    {        
        [TestMethod]
        public void ConstructingA4x4Matrix()
        {
            var mat = new rt.Matrix(new List<double>{
                1,2,3,4,
                5.5f,6.5f,7.5f,8.5f,
                9,10,11,12,
                13.5f,14.5f,15.5f,16.5f
                });


            Assert.AreEqual(4, mat.Width);
            Assert.AreEqual(4, mat.Height);
            Assert.AreEqual(1.0f, mat[0, 0]);
            Assert.AreEqual(4.0f, mat[0, 3]);
            Assert.AreEqual(5.5f, mat[1, 0]);
            Assert.AreEqual(7.5f, mat[1, 2]);
            Assert.AreEqual(11.0f, mat[2, 2]);
            Assert.AreEqual(13.5f, mat[3, 0]);
            Assert.AreEqual(15.5f, mat[3, 2]);
        }

        [TestMethod]
        public void A2DMatrixShouldBeRepresentable()
        {
            var mat = new Matrix(new List<double>{
                -3,5,
                1,-2

                });

            Assert.AreEqual(2, mat.Width);
            Assert.AreEqual(2, mat.Height);
            Assert.AreEqual(-3.0f, mat[0, 0]);
            Assert.AreEqual(5.0f, mat[0, 1]);
            Assert.AreEqual(1.0f, mat[1, 0]);
            Assert.AreEqual(-2.0f, mat[1, 1]);
        }

        [TestMethod]
        public void A3DMatrixShouldBeRepresentable()
        {
            var mat = new Matrix(new List<double>{
                -3,5,0,
                1,-2,-7,
                0,1,1

                });

            Assert.AreEqual(3, mat.Width);
            Assert.AreEqual(3, mat.Height);
            Assert.AreEqual(-3.0f, mat[0, 0]);
            Assert.AreEqual(-2.0f, mat[1, 1]);
            Assert.AreEqual(1.0f, mat[2, 2]);
        }

        [TestMethod]
        public void MatrixEqualityWithIdenticalMatrices()
        {
            var matA = new Matrix(new List<double>{
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16

                });
            var matB = new Matrix(new List<double>{
                1, 2, 3, 4,
                5, 6, 7, 8,
                9, 10, 11, 12,
                13, 14, 15, 16

                });
            Assert.AreEqual(matA, matB);
        }

        [TestMethod]
        public void MatrixEqualityWithDifferentMatrices()
        {
            var matA = new Matrix(new List<double>{
                1, 2, 3, 4,
                5, 6, 7, 8,
                8, 7, 6, 5,
                4, 3, 2, 1
                });
            var matB = new Matrix(new List<double>{
                2, 3, 4, 5,
                6, 7, 8, 9,
                9, 8, 7, 6,
                5, 4, 3, 2
                });
            Assert.AreNotEqual(matA, matB);
        }

        [TestMethod]
        public void MultiplyingTwoMatrices()
        {
            var matA = new Matrix(new List<double>{
                1,2,3,4,
                5,6,7,8,
                9,8,7,6,
                5,4,3,2

            });
            var matB = new Matrix(new List<double>{
                -2,1,2,3,
                3,2,1,-1,
                4,3,6,5,
                1,2,7,8

            });

            var exp = new Matrix(new List<double>{
                20, 22, 50, 48,
                44, 54, 114, 108,
                40, 58, 110, 102,
                16, 26, 46, 42

            });
            var matC = matA * matB;
            Assert.AreEqual(exp, matC);
        }

        [TestMethod]
        public void MultiplyByATuple()
        {
            var matrix = new Matrix(new List<double>{
                1,2,3,4,
                2,4,4,2,
                8,6,4,1,
                0,0,0,1
                });

            var tuple = new Tuple(1, 2, 3, 1);
            Assert.AreEqual(new Tuple(18, 24, 33, 1), matrix * tuple);
        }

        [TestMethod]
        public void MultiplyByIdentityMatrix()
        {
            var matrix = new Matrix(new List<double> {
                0,1,2,4,
                1,2,4,8,
                2,4,8,16,
                4,8,16,32

                });

            Assert.AreEqual(matrix, matrix * Matrix.Identity(4));
        }

        [TestMethod]
        public void MultiplyIdentityMatrixByTuple()
        {
            var tuple = new Tuple(1, 2, 3, 4);
            Assert.AreEqual(tuple, Matrix.Identity(4) * tuple);
        }

        [TestMethod]
        public void TransposeAMatrix()
        {
            var a = new Matrix(new List<double>{
                0,9,3,0,
                9,8,0,8,
                1,8,5,3,
                0,0,5,8

            });
            var b = new Matrix(new List<double>{
                0,9,1,0,
                9,8,8,0,
                3,0,5,5,
                0,8,3,8

            });
            Assert.AreEqual(b, a.Transpose());
        }

        [TestMethod]
        public void DeterminantOf2x2()
        {
            var m = new Matrix(new List<double>{
                1,5,
                -3,2

                });
            Assert.AreEqual(17.0f, m.Determinant());
        }
        /*
        public void SubmatrixOf3x3) {
            var m = rt.matrix3x3({
                1,5,0,
                -3,2,7,
                0,6,-3});

            Assert.AreEqual(rt.matrix2x2({
                -3,2,
                0,6 }), m.submatrix(0, 2));
        }

        public void SubmatrixOf4x4) {
            var m = rt.matrix4x4({
                -6,1,1,6,
                -8,5,8,6,
                -1,0,8,2,
                -7,1,-1,1
            });

            Assert.AreEqual(rt.matrix3x3({
                -6,1,6,
                -8,8,6,
                -7,-1,1 }), m.submatrix(2, 1));
        }
        */

        [TestMethod]
        public void Minor3x3()
        {
            var m = new Matrix(new List<double>{
                1,2,3,
                2,3,4,
                4,5,6 });

            Assert.AreEqual(new Matrix(new List<double> {
                3,4,
                5,6 }), m.Minor(0, 0));

            Assert.AreEqual(new Matrix(new List<double> {
                1,3,
                4,6 }), m.Minor(1, 1));

            Assert.AreEqual(new Matrix(new List<double> {
                1,2,
                2,3 }), m.Minor(2, 2));
        }


        [TestMethod]
        public void Cofactor3x3()
        {
            var m = new Matrix(new List<double> {
                1,2,3,
                2,3,1,
                3,1,2 });

            Assert.AreEqual(new Matrix(new List<double> {
                5,-1,-7,
                -1,-7,5,
                -7,5,-1 }), m.Cofactor());
        }

        [TestMethod]
        public void Determinant3x3()
        {
            var m = new Matrix(new List<double>{
                1, 2, 3,
                2, 3, 1,
                3, 1, 2 });
            Assert.AreEqual(-18.0f, m.Determinant());
        }

        [TestMethod]
        public void Determinant4x4()
        {
            var m = new Matrix(new List<double>{
                1,2,3,4,
                2,3,4,1,
                3,4,1,2,
                4,1,2,3 });
            Assert.AreEqual(160.0f, m.Determinant());
        }

        [TestMethod]
        public void MatrixIsInvertible()
        {
            var m = new Matrix(new List<double>{
                6,4,4,4,
                5,5,7,6,
                4,-9,3,-7,
                9,1,7,-6

                });
            Assert.AreEqual(-2120.0f, m.Determinant());
            var b = m.Inverse();
        }

        [TestMethod]
        public void MatrixIsNotInvertible()
        {
            var m = new Matrix(new List<double>{
                -4,2,-2,-2,
                9,6,2,6,
                0,-5,1,-5,
                0,0,0,0

                });
            Assert.AreEqual(0.0f, m.Determinant());
            Assert.ThrowsException<System.ApplicationException>(() => m.Inverse());
        }

        [TestMethod]
        public void CalculateTheInverseOfAMatrix()
        {
            var a = new Matrix(new List<double>{
                -5,2,6,-8,
                1,-5,1,8,
                7,7,-6, -7,
                1,-3,7,4

                });
            var b = a.Inverse();
            Assert.AreEqual(532.0f, a.Determinant(), EPSILON);
            //Assert.AreEqual(-160.f, a.cofactor(2, 3));
            Assert.AreEqual(-160.0f / 532, b[3, 2], EPSILON);
            //Assert.AreEqual(105.f, a.cofactor(3, 2));
            Assert.AreEqual(105.0f / 532, b[2, 3], EPSILON);

            Assert.AreEqual(new Matrix(new List<double>{
                0.21805f, 0.45113f, 0.24060f, -0.04511f,
                -0.80827f, -1.45677f, -0.44361f, 0.52068f,
                -0.07895f, -0.22368f, -0.05263f, 0.19737f,
                -0.52256f, -0.81391f, -0.30075f, 0.30639f }), b);
        }

        [TestMethod]
        public void CalculatingTheInverseOfAnotherMatrix()
        {

            var a = new Matrix(new List<double>{
                8, -5, 9, 2,
                7, 5, 6, 1,
                -6, 0, 9, 6,
                -3, 0, -9, -4

                });

            var b = new Matrix(new List<double>{
                -0.15385f, -0.15385f, -0.28205f, -0.53846f,
                -0.07692f, 0.12308f, 0.02564f, 0.03077f,
                0.35897f, 0.35897f, 0.43590f, 0.92308f,
                -0.69231f, -0.69231f, -0.76923f, -1.92308f
                });

            Assert.AreEqual(b, a.Inverse());
        }

        [TestMethod]
        public void CalculatingTheInverseOfAThirdMatrix()
        {
            var a = new Matrix(new List<double>{
                9, 3, 0, 9,
                -5, -2, -6, -3,
                -4, 9, 6, 4,
                -7, 6, 6, 2

                });
            var b = new Matrix(new List<double>{
                -0.04074f, -0.07778f, 0.14444f, -0.22222f,
                -0.07778f, 0.03333f, 0.36667f, -0.33333f,
                -0.02901f, -0.14630f, -0.10926f, 0.12963f,
                 0.17778f, 0.06667f, -0.26667f, 0.33333f,
                });
            Assert.AreEqual(b, a.Inverse());
        }

        [TestMethod]
        public void MultiplyingAProductByItsInverse()
        {
            var a = new Matrix(new List<double>{
                3, -9, 7, 3,
				3, -8, 2, -9,
				-4, 4, 4, 1,
				-6, 5, -1, 1,
				});
            var b = new Matrix(new List<double>{
                8, 2, 2, 2,
				3, -1, 7, 0,
				7, 0, 5, 4,
				6, -2, 0, 5,
				});
            var c = a * b;
            Assert.AreEqual(a, c * b.Inverse());
        }
    }
}
