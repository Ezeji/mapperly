﻿using Riok.Mapperly.Abstractions;
using Riok.Mapperly.Diagnostics;

namespace Riok.Mapperly.Tests.Mapping;

[UsesVerify]
public class IgnoreObsoleteTest
{
    private const string _classA = """
        class A
        {
            public int Value { get; set; }

            [Obsolete]
            public int Ignored { get; set; }
        }
        """;

    private const string _classB = """
        class B
        {
            public int Value { get; set; }

            [Obsolete]
            public int Ignored { get; set; }
        }
        """;

    [Fact]
    public void ClassAttributeIgnoreObsoleteNone()
    {
        var source = TestSourceBuilder.Mapping(
            "A",
            "B",
            TestSourceBuilderOptions.WithIgnoreObsolete(IgnoreObsoleteMembersStrategy.None),
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                target.Ignored = source.Ignored;
                return target;
                """
            );
    }

    [Fact]
    public void ClassAttributeIgnoreObsoleteBoth()
    {
        var source = TestSourceBuilder.Mapping(
            "A",
            "B",
            TestSourceBuilderOptions.WithIgnoreObsolete(IgnoreObsoleteMembersStrategy.Both),
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                return target;
                """
            );
    }

    [Fact]
    public void ClassAttributeIgnoreSourceShouldDiagnostic()
    {
        var source = TestSourceBuilder.Mapping(
            "A",
            "B",
            TestSourceBuilderOptions.WithIgnoreObsolete(IgnoreObsoleteMembersStrategy.Source),
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                return target;
                """
            )
            .HaveDiagnostic(DiagnosticDescriptors.SourceMemberNotFound);
    }

    [Fact]
    public void ClassAttributeIgnoreTargetShouldDiagnostic()
    {
        var source = TestSourceBuilder.Mapping(
            "A",
            "B",
            TestSourceBuilderOptions.WithIgnoreObsolete(IgnoreObsoleteMembersStrategy.Target),
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                return target;
                """
            )
            .HaveDiagnostic(DiagnosticDescriptors.SourceMemberNotMapped);
    }

    [Fact]
    public void MethodAttributeIgnoreObsoleteNone()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            "[MapperIgnoreObsoleteMembers(IgnoreObsoleteMembersStrategy.None)] partial B Map(A source);",
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                target.Ignored = source.Ignored;
                return target;
                """
            );
    }

    [Fact]
    public void MethodAttributeIgnoreObsoleteBoth()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes("[MapperIgnoreObsoleteMembers] partial B Map(A source);", _classA, _classB);

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                return target;
                """
            );
    }

    [Fact]
    public void MethodAttributeIgnoreObsoleteSourceShouldDiagnostic()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            "[MapperIgnoreObsoleteMembers(IgnoreObsoleteMembersStrategy.Source)] partial B Map(A source);",
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                return target;
                """
            )
            .HaveDiagnostic(DiagnosticDescriptors.SourceMemberNotFound);
    }

    [Fact]
    public void MethodAttributeIgnoreObsoleteTargetShouldDiagnostic()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            "[MapperIgnoreObsoleteMembers(IgnoreObsoleteMembersStrategy.Target)] partial B Map(A source);",
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                return target;
                """
            )
            .HaveDiagnostic(DiagnosticDescriptors.SourceMemberNotMapped);
    }

    [Fact]
    public void MethodAttributeOverridesClassAttribute()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            "[MapperIgnoreObsoleteMembers(IgnoreObsoleteMembersStrategy.None)] partial B Map(A source);",
            TestSourceBuilderOptions.WithIgnoreObsolete(IgnoreObsoleteMembersStrategy.Both),
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                target.Ignored = source.Ignored;
                return target;
                """
            );
    }

    [Fact]
    public void MapPropertyOverridesIgnoreObsoleteBoth()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            """
                [MapProperty("Ignored", "Ignored")]
                [MapperIgnoreObsoleteMembers]
                partial B Map(A source);
                """,
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                target.Ignored = source.Ignored;
                return target;
                """
            );
    }

    [Fact]
    public void MapPropertyOverridesIgnoreObsoleteSource()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            """
                [MapProperty("Ignored", "Ignored")]
                [MapperIgnoreObsoleteMembers(IgnoreObsoleteMembersStrategy.Source)]
                partial B Map(A source);
                """,
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source, TestHelperOptions.AllowInfoDiagnostics)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                target.Ignored = source.Ignored;
                return target;
                """
            );
    }

    [Fact]
    public void MapPropertyOverridesIgnoreObsoleteTarget()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            """
                [MapProperty("Ignored", "Ignored")]
                [MapperIgnoreObsoleteMembers(IgnoreObsoleteMembersStrategy.Target)]
                partial B Map(A source);
                """,
            _classA,
            _classB
        );

        TestHelper
            .GenerateMapper(source)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B();
                target.Value = source.Value;
                target.Ignored = source.Ignored;
                return target;
                """
            );
    }

    [Fact]
    public void MapInitPropertyWhenIgnoreObsoleteTarget()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            """
                [MapProperty("Ignored", "Ignored")]
                [MapperIgnoreObsoleteMembers(IgnoreObsoleteMembersStrategy.Target)]
                partial B Map(A source);
                """,
            _classA,
            """
                class B
                {
                    public int Value { get; set; }

                    [Obsolete]
                    public int Ignored { get; init; }
                }
                """
        );

        TestHelper
            .GenerateMapper(source)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B()
                {
                    Ignored = source.Ignored
                };
                target.Value = source.Value;
                return target;
                """
            );
    }

    [Fact]
    public void MapRequiredPropertyWhenIgnoreObsoleteTarget()
    {
        var source = TestSourceBuilder.MapperWithBodyAndTypes(
            """
                [MapProperty("Ignored", "Ignored")]
                [MapperIgnoreObsoleteMembers(IgnoreObsoleteMembersStrategy.Target)]
                partial B Map(A source);
                """,
            _classA,
            """
                class B
                {
                    public int Value { get; set; }

                    [Obsolete]
                    public required int Ignored { get; set; }
                }
                """
        );

        TestHelper
            .GenerateMapper(source)
            .Should()
            .HaveSingleMethodBody(
                """
                var target = new global::B()
                {
                    Ignored = source.Ignored
                };
                target.Value = source.Value;
                return target;
                """
            );
    }
}
