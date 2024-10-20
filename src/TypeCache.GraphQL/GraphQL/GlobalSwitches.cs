using System.Collections.Generic;
using System.ComponentModel;
using GraphQL.Conversion;
using GraphQL.Utilities;
using TypeCache.GraphQL.Types;

namespace GraphQL;

// decorate any test classes which contain tests that modify these global switches with
//   [Collection("StaticTests")]
// these tests will run sequentially after all other tests are complete
// be sure to restore the global switch to its initial state with a finally block

/// <summary>
/// Global options for configuring GraphQL execution.
/// </summary>
public static class GlobalSwitches
{
    /// <summary>
    /// Enables or disables setting default values for 'defaultValue' from <see cref="DefaultValueAttribute"/>.
    /// <br/>
    /// By default enabled.
    /// </summary>
    public static bool EnableReadDefaultValueFromAttributes { get; set; } = true;

    /// <summary>
    /// Enables or disables setting default values for 'deprecationReason' from <see cref="ObsoleteAttribute"/>.
    /// <br/>
    /// By default enabled.
    /// </summary>
    public static bool EnableReadDeprecationReasonFromAttributes { get; set; } = true;

    /// <summary>
    /// Enables or disables setting default values for 'description' from <see cref="DescriptionAttribute"/>.
    /// <br/>
    /// By default enabled.
    /// </summary>
    public static bool EnableReadDescriptionFromAttributes { get; set; } = true;

    /// <summary>
    /// Enables or disables setting default values for 'description' from XML documentation.
    /// <br/>
    /// By default disabled.
    /// </summary>
    public static bool EnableReadDescriptionFromXmlDocumentation { get; set; } = false;

    /// <summary>
    /// Gets or sets current validation delegate. By default this delegate validates all names according
    /// to the GraphQL <see href="https://spec.graphql.org/October2021/#sec-Names">specification</see>.
    /// <br/><br/>
    /// Setting this delegate allows you to use names not conforming to the specification, for example
    /// 'enum-member'. Only change it when absolutely necessary. This is typically only overridden
    /// when implementing a custom <see cref="INameConverter"/> that fixes names, making them spec-compliant.
    /// <br/><br/>
    /// Keep in mind that regardless of this setting, names are validated upon schema initialization,
    /// after being processed by the <see cref="INameConverter"/>. This is due to the fact that the
    /// parser cannot parse incoming queries with invalid characters in the names, so the resulting
    /// member would become unusable.
    /// </summary>
    public static Action<string, NamedElement> NameValidation = NameValidator.ValidateDefault;

    /// <summary>
    /// Enable this switch to see additional debugging information in exception messages during schema initialization.
    /// <br/>
    /// By default disabled.
    /// </summary>
    public static bool TrackGraphTypeInitialization { get; set; } = false;

    /// <summary>
    /// Enables caching of reflection metadata and resolvers from <see cref="Types.AutoRegisteringObjectGraphType{TSourceType}">AutoRegisteringObjectGraphType</see>;
    /// useful for scoped schemas.
    /// <br/><br/>
    /// By default disabled. <see cref="GraphQLBuilderExtensions.AddSchema{TSchema}(DI.IGraphQLBuilder, DI.ServiceLifetime)">AddSchema</see> sets
    /// this value to <see langword="true"/> when <see cref="DI.ServiceLifetime.Scoped"/> is specified.
    /// <br/><br/>
    /// Note that with reflection caching enabled, if there are two different classes derived from <see cref="Types.AutoRegisteringObjectGraphType{TSourceType}">AutoRegisteringObjectGraphType</see>
    /// that have the same TSourceType, one instance will incorrectly pull cached information stored by the other instance.
    /// </summary>
    public static bool EnableReflectionCaching { get; set; }
}
