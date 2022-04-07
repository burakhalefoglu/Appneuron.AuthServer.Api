using Core.Entities;

namespace Entities.Dtos;

/// <summary>
///     Simple selectable lists have been created to return with a single schema through the API.
/// </summary>
public class SelectionItem : IDto
{
    public SelectionItem()
    {
    }

    public SelectionItem(long id, string label)
    {
        Id = id;
        Label = label;
    }

    public long Id { get; set; }
    public string ParentId { get; set; }
    public string Label { get; set; }
    public bool IsDisabled { get; set; }
}