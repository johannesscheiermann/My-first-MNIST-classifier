using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions;

namespace MathHelper
{
    public sealed class Matrix
    {
        private readonly double[][] _values;
        private readonly Lazy<double[]> _lazyFlattedValues;

        public Matrix(double[][] values)
        {
            Dimensions = values
                .Require(
                    v => v.Length > 0 && v.First().Length > 0,
                    _ => "A matrix must be constructed from an non empty two dimensional array")
                .Let(it => (it.Length, it.First().Length));
            _values = values;
            _lazyFlattedValues = new Lazy<double[]>(() => _values.SelectMany(it => it).ToArray());
        }

        public (int Rows, int Columns) Dimensions { get; }

        public double Value(int row, int column) =>
            _values[
                row.Require(
                    it => it >= 0 && it < Dimensions.Rows,
                    it => $"Expected {nameof(row)} to be >= 0 and < {Dimensions.Rows}; Got {it}")
            ][
                column.Require(
                    it => it >= 0 && it < Dimensions.Columns,
                    it => $"Expected {nameof(column)} to be >= 0 and < {Dimensions.Columns}; Got {it}")
            ];

        public static Matrix Traverse(Matrix matrix)
        {
            var rows = Enumerable.Range(0, matrix.Dimensions.Rows).ToImmutableList();

            return Enumerable
                .Range(0, matrix.Dimensions.Columns)
                .SelectAsArray(
                    c => rows.SelectAsArray(
                        r => matrix.Value(r, c)
                    ))
                .AsMatrix();
        }

        public static Matrix Multiply(Matrix a, Matrix b)
        {
            if (a.Dimensions.Columns != b.Dimensions.Rows)
                throw new InvalidOperationException(
                    "Matrix multiplication cannot be performed due to incompatible dimensions.");

            var rowsOfA = a._values;
            var colsOfB = b.Traverse()._values;

            return rowsOfA.SelectAsArray(row =>
                colsOfB.SelectAsArray(col =>
                    row.EnsureSameLengthAndZipWith(col)
                        .Select(tuple => tuple.Item1 * tuple.Item2)
                        .Sum()
                )).AsMatrix();
        }


        public Matrix Map(Func<double, double> transform) =>
            _values.SelectAsArray(
                row => row.SelectAsArray(transform)
            ).AsMatrix();

        public Matrix Add(Matrix matrix)
        {
            if (Dimensions != matrix.Dimensions)
                throw new InvalidOperationException(
                    "Matrix addition cannot be performed due to different dimensions.");

            return _values.Zip(matrix._values).SelectAsArray(
                it => it.Item1.Zip(it.Item2)
                    .SelectAsArray(tuple => tuple.First + tuple.Second)
            ).AsMatrix();
        }

        #region generated

        private bool Equals(Matrix other) =>
            _lazyFlattedValues.Value.SequenceEqual(other._lazyFlattedValues.Value)
            && Dimensions.Equals(other.Dimensions);

        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Matrix other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(
                (_lazyFlattedValues.Value as IStructuralEquatable).GetHashCode(EqualityComparer<double>.Default),
                Dimensions
            );

        #endregion
    }

    public static class MatrixExtensions
    {
        public static Matrix AsMatrix(this double[][] @this) =>
            new Matrix(@this);

        public static Matrix AsMatrix(IEnumerable<IEnumerable<double>> @this) =>
            @this.SelectAsArray(it => it.ToArray()).AsMatrix();

        public static Matrix Traverse(this Matrix matrix) =>
            Matrix.Traverse(matrix);

        public static Matrix MultiplyWith(this Matrix a, Matrix b) =>
            Matrix.Multiply(a, b);
    }
}