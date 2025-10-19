using Mahamudra.Tapper.Tests.Brands;
using Mahamudra.Tapper.Tests.Categories;

namespace Mahamudra.Tapper.Tests.Products.Dtos;

public class BrandCategoryDto
{
    public IEnumerable<Brand>? Brands { get; set; }
    public IEnumerable<Category>? Categories { get; set; }
} 