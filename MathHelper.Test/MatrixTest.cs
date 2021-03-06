using System;
using NFluent;
using Xunit;

namespace MathHelper.Test
{
    public class MatrixTest
    {
        [Fact]
        public void Adopts_the_dimensions_of_the_two_dimensional_array() =>
            Check.That(
                new Matrix(new[]
                    {
                        new[] {1.0, 1.0, 1.0},
                        new[] {1.0, 1.0, 1.0}
                    }
                ).Dimensions
            ).Equals((2, 3));

        [Fact]
        public void Map_transforms_all_values_of_the_Matrix_based_on_the_passed_transformation_function() =>
            Check.That(
                new[]
                {
                    new[] {1.0, 2.0, 3.0},
                    new[] {4.0, 5.0, 6.0}
                }.AsMatrix().Map(value => value * 5.0)
            ).Equals(
                new[]
                {
                    new[] {5.0, 10.0, 15.0},
                    new[] {20.0, 25.0, 30.0}
                }.AsMatrix()
            );

        public sealed class Throws_if_a_Matrix_is
        {
            [Fact]
            public void constructed_from_an_empty_two_dimensional_array()
            {
                Check.ThatCode(
                    () => new Matrix(
                        Array.Empty<double[]>()
                    )).Throws<ArgumentException>().WithMessage(
                    "A matrix must be constructed from an non empty two dimensional array");

                Check.ThatCode(
                    () => new Matrix(
                        new[] {Array.Empty<double>()}
                    )).Throws<ArgumentException>().WithMessage(
                    "A matrix must be constructed from an non empty two dimensional array");
            }

            [Theory]
            [InlineData(-1, 0, true)]
            [InlineData(0, -1, true)]
            [InlineData(0, 0, false)]
            [InlineData(0, 1, false)]
            [InlineData(0, 2, false)]
            [InlineData(1, 0, false)]
            [InlineData(1, 1, false)]
            [InlineData(1, 2, false)]
            [InlineData(1, 3, true)]
            [InlineData(2, 0, true)]
            public void accessed_outside_its_bounds(int row, int column, bool exceptionExpected)
            {
                var check = Check.ThatCode(
                    () =>
                        new Matrix(new[]
                            {
                                new[] {1.0, 2.0, 3.0},
                                new[] {4.0, 5.0, 6.0}
                            }
                        ).Value(row: row, column: column)
                );

                if (exceptionExpected)
                    check.ThrowsAny();
                else
                    check.DoesNotThrow();
            }
        }

        public sealed class Traversing_a_Matrix
        {
            [Fact]
            public void flips_its_dimensions() =>
                Check.That(
                    new Matrix(new[]
                        {
                            new[] {1.0, 1.0, 1.0},
                            new[] {1.0, 1.0, 1.0}
                        }
                    ).Traverse().Dimensions
                ).Equals((3, 2));

            [Fact]
            public void converts_its_rows_into_columns_and_vice_versa() =>
                Check.That(
                    new Matrix(new[]
                        {
                            new[] {1.0, 2.0, 3.0},
                            new[] {4.0, 5.0, 6.0}
                        }
                    ).Traverse()
                ).Equals(
                    new Matrix(new[]
                    {
                        new[] {1.0, 4.0},
                        new[] {2.0, 5.0},
                        new[] {3.0, 6.0},
                    })
                );
        }

        public sealed class When_multiplying_two_matrices
        {
            [Fact]
            public void the_counts_of_the_first_Matrixs_columns_and_the_second_Matrixs_rows_must_be_equal() =>
                Check.ThatCode(
                        () =>
                            new Matrix(new[]
                                {
                                    new[] {1.0, 2.0, 3.0},
                                    new[] {4.0, 5.0, 6.0}
                                }
                            ).MultiplyWith(new Matrix(new[]
                                {
                                    new[] {1.0, 2.0, 3.0},
                                    new[] {4.0, 5.0, 6.0}
                                }
                            )))
                    .Throws<InvalidOperationException>()
                    .WithMessage("Matrix multiplication cannot be performed due to incompatible dimensions.");

            [Fact]
            public void the_resulting_Matrix_has_the_same_count_of_rows_as_the_first_Matrix() =>
                Check.That(
                    new Matrix(new[]
                        {
                            new[] {1.0, 2.0, 3.0},
                            new[] {4.0, 5.0, 6.0}
                        }
                    ).MultiplyWith(new Matrix(new[]
                        {
                            new[] {1.0, 2.0},
                            new[] {3.0, 4.0},
                            new[] {5.0, 6.0}
                        }
                    )).Dimensions.Rows
                ).Equals(2);

            [Fact]
            public void the_resulting_Matrix_has_the_same_count_of_columns_as_the_second_Matrix() =>
                Check.That(
                    new Matrix(new[]
                        {
                            new[] {1.0, 2.0, 3.0},
                            new[] {4.0, 5.0, 6.0}
                        }
                    ).MultiplyWith(new Matrix(new[]
                        {
                            new[] {1.0, 2.0},
                            new[] {3.0, 4.0},
                            new[] {5.0, 6.0}
                        }
                    )).Dimensions.Columns
                ).Equals(2);

            [Fact]
            public void
                each_row_of_the_first_Matrix_is_multiplied_and_summed_up_with_each_column_of_the_second_Matrix()
            {
                var a = new Matrix(new[]
                {
                    new[] {1.0, 2.0, 3.0},
                    new[] {4.0, 5.0, 6.0}
                });

                var b = new Matrix(new[]
                    {
                        new[] {1.0, 2.0},
                        new[] {3.0, 4.0},
                        new[] {5.0, 6.0}
                    }
                );

                Check.That(
                    a.MultiplyWith(b)
                ).Equals(
                    new Matrix(new[]
                    {
                        new[] {22.0, 28.0},
                        new[] {49.0, 64.0}
                    })
                );
            }
        }

        public sealed class When_adding_two_matrices
        {
            [Fact]
            public void the_dimensions_of_both_matrices_must_be_equal() =>
                Check.ThatCode(
                        () =>
                            new Matrix(new[]
                                {
                                    new[] {1.0, 2.0, 3.0},
                                    new[] {4.0, 5.0, 6.0}
                                }
                            ).Add(new Matrix(new[]
                                {
                                    new[] {1.0, 2.0, 3.0},
                                }
                            )))
                    .Throws<InvalidOperationException>()
                    .WithMessage("Matrix addition cannot be performed due to different dimensions.");

            [Fact]
            public void the_resulting_Matrix_has_the_same_dimensions_as_both_summand_matrices() =>
                Check.That(
                    new Matrix(new[]
                        {
                            new[] {1.0, 2.0, 3.0},
                            new[] {4.0, 5.0, 6.0}
                        }
                    ).Add(new Matrix(new[]
                        {
                            new[] {1.0, 2.0, 3.0},
                            new[] {4.0, 5.0, 6.0}
                        }
                    )).Dimensions
                ).Equals((2, 3));

            [Fact]
            public void each_value_of_a_Matrix_is_summed_up_with_its_counter_value_of_the_other_Matrix()
            {
                var a = new Matrix(new[]
                {
                    new[] {1.0, 2.0, 3.0},
                    new[] {4.0, 5.0, 6.0}
                });

                var b = new Matrix(new[]
                    {
                        new[] {3.0, 2.4, 2.0},
                        new[] {6.0, 2.0, 16.0}
                    }
                );

                Check.That(
                    a.Add(b)
                ).Equals(
                    new Matrix(new[]
                    {
                        new[] {4.0, 4.4, 5.0},
                        new[] {10.0, 7.0, 22.0}
                    })
                );
            }
        }
    }
}