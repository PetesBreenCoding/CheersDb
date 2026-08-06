namespace CheersDb.Api.Http;

/// <summary>
/// Defines standard link relationship types for hypermedia links in the API responses, following common conventions for RESTful APIs.
/// </summary>
public static class LinkRels
{
	/// <summary>
	/// Indicates an alternate representation of the resource.
	/// </summary>
	public const string Alternate = "alternate";

	/// <summary>
	/// Identifies a collection resource (a list of items).
	/// </summary>
	public const string Collection = "collection";

	/// <summary>
	/// Identifies an individual item within a collection.
	/// </summary>
	public const string Item = "item";

	/// <summary>
	/// A link to the next page or resource in a sequence.
	/// </summary>
	public const string Next = "next";

	/// <summary>
	/// A link to the previous page or resource in a sequence.
	/// </summary>
	public const string Prev = "prev";

	/// <summary>
	/// A related resource linked to the current resource.
	/// </summary>
	public const string Related = "related";

	/// <summary>
	/// A link that points to the resource itself.
	/// </summary>
	public const string Self = "self";

	/// <summary>
	/// A link to a parent resource or higher-level context.
	/// </summary>
	public const string Up = "up";
}