using System.Globalization;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace PublicWeb.Tests;

public class LegacyReviewReproductionTests : IClassFixture<ReviewDatabase>
{
    private readonly ReviewDatabase database;
    public LegacyReviewReproductionTests(ReviewDatabase database) => this.database = database;

    [Fact]
    public async Task Empty_optional_photo_binds_null_and_legacy_persistence_throws_SQL_515_before_insert()
    {
        database.Clear();
        var comment = await BindLegacyForm("");
        Assert.Null(comment.ImageUrl);
        // Isolates the old action's post-CAPTCHA branch without sending a Google request.
        var error = Assert.Throws<DbUpdateException>(() => new CommentManager(new EfCommentRepository()).TAdd(comment));
        var sql = Assert.IsType<SqlException>(error.InnerException);
        Assert.Equal(515, sql.Number);
        Assert.Contains("ImageUrl", sql.Message);
        Assert.Equal(0, database.Count);
    }

    [Fact]
    public async Task Same_legacy_form_with_a_photo_inserts_one_row()
    {
        database.Clear();
        var comment = await BindLegacyForm("/canabicom/profilePhoto/test.jpg");
        new CommentManager(new EfCommentRepository()).TAdd(comment);
        Assert.Equal(1, database.Count);
    }

    private static async Task<Comment> BindLegacyForm(string photo)
    {
        var services = new ServiceCollection().AddLogging();
        services.AddControllersWithViews();
        using var provider = services.BuildServiceProvider();
        var metadata = provider.GetRequiredService<IModelMetadataProvider>().GetMetadataForType(typeof(Comment));
        var factory = provider.GetRequiredService<IModelBinderFactory>();
        var binder = factory.CreateBinder(new ModelBinderFactoryContext { Metadata = metadata, CacheToken = typeof(Comment) });
        var form = new FormCollection(new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase) {
            ["CommentUserName"] = "Yerel test", ["CommentTitle"] = "Yalnız test",
            ["CommentContent"] = "Gerçek müşteri yorumu değildir.", ["ImageURL"] = photo
        });
        var http = new DefaultHttpContext { RequestServices = provider };
        var action = new ActionContext(http, new RouteData(), new ActionDescriptor());
        var context = DefaultModelBindingContext.CreateBindingContext(action,
            new FormValueProvider(BindingSource.Form, form, CultureInfo.InvariantCulture), metadata, null, "");
        await binder.BindModelAsync(context);
        var comment = Assert.IsType<Comment>(context.Result.Model);
        comment.CommentDate = DateTime.Today;
        comment.CommentStatus = true;
        return comment;
    }
}
