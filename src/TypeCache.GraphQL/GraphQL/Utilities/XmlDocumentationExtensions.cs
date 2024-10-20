using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GraphQL.Utilities;

/// <summary>
/// Provides extension methods for reading XML comments from reflected members.
/// </summary>
internal static class XmlDocumentationExtensions
{
	private static readonly ConcurrentDictionary<string, XDocument?> _cachedXml = new(StringComparer.OrdinalIgnoreCase);

	private static string GetParameterName(this ParameterInfo parameter)
		=> GetTypeName(parameter.ParameterType);

	private static string? NullIfEmpty(this string? text) => text == string.Empty ? null : text;

	private static string GetTypeName(Type type)
	{
		if (type.IsGenericType)
		{
			string baseName = type.GetGenericTypeDefinition().ToString();
			baseName = baseName.Substring(0, baseName.IndexOf('`'));
			return $"{baseName}{{{string.Join(',', type.GetGenericArguments().Select(GetTypeName))}}}";
		}

		return type.FullName!;
	}

	/// <summary>
	/// Returns the expected name for a member element in the XML documentation file.
	/// </summary>
	/// <param name="memberInfo">The reflected member.</param>
	/// <returns>The name of the member element.</returns>
	private static string GetMemberElementName(MemberInfo memberInfo)
	{
		return memberInfo switch
		{
			Type type => $"T{type.FullName}",
			ConstructorInfo constructorInfo => $"M{constructorInfo.DeclaringType!.FullName}.{constructorInfo.Name.Replace(".ctor", "#ctor")}({getParameters(constructorInfo)})",
			MethodInfo methodInfo => $"M{methodInfo.DeclaringType!.FullName}.{methodInfo.Name}({getParameters(methodInfo)})",
			PropertyInfo propertyInfo => $"P{propertyInfo.DeclaringType!.FullName}.{propertyInfo.Name}",
			FieldInfo fieldInfo => $"F{fieldInfo.DeclaringType!.FullName}.{fieldInfo.Name}",
			EventInfo eventInfo => $"E{eventInfo.DeclaringType!.FullName}.{eventInfo.Name}",
			_ => throw new UnreachableException($"Unknown member type: {memberInfo.MemberType:F}")
		};

		static string getParameters(MethodBase method)
			=> string.Join(',', method.GetParameters().Select(_ => _.GetParameterName()));
	}

	private static XDocument? GetDocument(Assembly assembly, string pathToXmlFile)
	{
		return _cachedXml.GetOrAdd(assembly.GetName().FullName, key => getDocument(assembly, pathToXmlFile));

		static XDocument? getDocument(Assembly assembly, string pathToXmlFile)
		{
			try
			{
				if (File.Exists(pathToXmlFile))
					return XDocument.Load(pathToXmlFile);

				var relativePath = Path.Combine(Path.GetDirectoryName(assembly.Location)!, pathToXmlFile);
				if (File.Exists(relativePath))
					return XDocument.Load(relativePath);
			}
			catch (Exception)
			{
				// No logging is needed
			}

			return null;
		}
	}

	/// <summary>
	/// Returns the XML documentation (summary tag) for the specified member.
	/// </summary>
	/// <param name="member">The reflected member.</param>
	/// <returns>The contents of the summary tag for the member.</returns>
	public static string? GetXmlDocumentation(this MemberInfo member)
		=> GetXmlDocumentation(member, member.Module.Assembly.GetName().Name + ".xml");

	/// <summary>
	/// Returns the XML documentation (summary tag) for the specified member.
	/// </summary>
	/// <param name="member">The reflected member.</param>
	/// <param name="pathToXmlFile">Path to the XML documentation file.</param>
	/// <returns>The contents of the summary tag for the member.</returns>
	public static string? GetXmlDocumentation(this MemberInfo member, string pathToXmlFile)
		=> GetXmlDocumentation(member, GetDocument(member.Module.Assembly, pathToXmlFile));

	/// <summary>
	/// Returns the XML documentation (summary tag) for the specified member.
	/// </summary>
	/// <param name="member">The reflected member.</param>
	/// <param name="xml">XML documentation.</param>
	/// <returns>The contents of the summary tag for the member.</returns>
	public static string? GetXmlDocumentation(this MemberInfo member, XDocument? xml)
		=> xml?.XPathEvaluate($"string(/doc/members/member[@name='{GetMemberElementName(member)}']/summary)").ToString()!.Trim().NullIfEmpty();

	/// <summary>
	/// Returns the XML documentation (returns/param tag) for the specified parameter.
	/// </summary>
	/// <param name="parameter">The reflected parameter (or return value).</param>
	/// <returns>The contents of the returns/param tag for the parameter.</returns>
	public static string? GetXmlDocumentation(this ParameterInfo parameter)
		=> GetXmlDocumentation(parameter, parameter.Member.Module.Assembly.GetName().Name + ".xml");

	/// <summary>
	/// Returns the XML documentation (returns/param tag) for the specified parameter.
	/// </summary>
	/// <param name="parameter">The reflected parameter (or return value).</param>
	/// <param name="pathToXmlFile">Path to the XML documentation file.</param>
	/// <returns>The contents of the returns/param tag for the parameter.</returns>
	public static string? GetXmlDocumentation(this ParameterInfo parameter, string pathToXmlFile)
		=> GetXmlDocumentation(parameter, GetDocument(parameter.Member.Module.Assembly, pathToXmlFile));

	/// <summary>
	/// Returns the XML documentation (returns/param tag) for the specified parameter.
	/// </summary>
	/// <param name="parameter">The reflected parameter (or return value).</param>
	/// <param name="xml">XML documentation.</param>
	/// <returns>The contents of the returns/param tag for the parameter.</returns>
	public static string? GetXmlDocumentation(this ParameterInfo parameter, XDocument? xml)
		=> parameter.IsRetval || string.IsNullOrEmpty(parameter.Name)
			? xml?.XPathEvaluate($"string(/doc/members/member[@name='{GetMemberElementName(parameter.Member)}']/returns)").ToString()!.Trim().NullIfEmpty()
			: xml?.XPathEvaluate($"string(/doc/members/member[@name='{GetMemberElementName(parameter.Member)}']/param[@name='{parameter.Name}'])").ToString()!.Trim().NullIfEmpty();
}
