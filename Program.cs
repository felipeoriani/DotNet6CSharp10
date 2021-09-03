// ---------------------------------------------------------------------------------------
// Hi, this is the Minimal in-memory Blog Api created using .NET 6 features!
// The target is to explore new features around C#, .net 6 and asp.net 6.
// Hope you enjoy it.
// ---------------------------------------------------------------------------------------

// ---------------------------------------------------------------------------------------
// Setup
// ---------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PostRepository>(new PostRepository()
{
	new (Guid.NewGuid(), "Performance Improvements in .NET 6", new(2021, 8, 1), "Microsoft", "See in this post some new performance improvements on .Net 6.", new List<Comment>() { new(Guid.NewGuid(), DateTime.Now, "Gordon", "Nice job", 4), new(Guid.NewGuid(), DateTime.Now, "Anne", "Good results.", 2), new(Guid.NewGuid(), DateTime.Now, "Paul", "Seems to be great", 3) }),
	new (Guid.NewGuid(), "Preview Features of .NET 6", new(2021, 6, 15), "Felipe", "See in this post some new features on .Net 6.", new List<Comment>() { new(Guid.NewGuid(), DateTime.Now, "John", "Nice new features!", 5), new(Guid.NewGuid(), DateTime.Now, "Mary", "It is getting better now!", 1) }),
	new (Guid.NewGuid(), "Thunder Cats will save the world", new(1995, 4, 20), "Lion", "Thunder, Thunder, Thunder, Thunder Cats ... ooooh!!!", new List<Comment>())
});

// ---------------------------------------------------------------------------------------
// Api
// ---------------------------------------------------------------------------------------

var app = builder.Build();

// health check
app.MapGet("/", () => Results.Ok());

// list the head of all the posts
app.MapGet("/posts", ([FromServices] PostRepository repo)
	=> repo.Select(x => new { x.Id, x.Title, x.Date, x.Author, Uri = $"/posts/{x.Id}" /*HATEOAS?*/ }));

// get a post by given id
app.MapGet("/posts/{id}", ([FromServices] PostRepository repo, Guid id) =>
{
	var post = repo.FirstOrDefault(x => x.Id == id);
	return post != null
		? Results.Ok(post)
		: Results.NotFound();
});

// create a new post
app.MapPost("/posts", ([FromServices] PostRepository repo, PostModel model) =>
{
	Post post = new(Guid.NewGuid(), model.Title, DateTime.Now, model.Author, model.Content, new List<Comment>());
	repo.Add(post);

	return Results.Created($"/posts/{post.Id}", post);
});

// patch a post by given id
// No MapPatch method yet ;(
app.MapMethods("/posts/{id}", new[] { "patch" }, ([FromServices] PostRepository repo, Guid id, PostModel model) => 
{
	var post = repo.FirstOrDefault(x => x.Id == id);
	if (post == null)
		return Results.NotFound();

	int index = repo.IndexOf(post);
	repo[index] = new(id, model.Title, post.Date, model.Author, model.Content, post.Comments);

	return Results.Ok(post);
});

// delete a post gy given id
app.MapDelete("/posts/{id}", ([FromServices] PostRepository repo, Guid id) =>
{
	repo.RemoveAll(x => x.Id == id);
	return Results.NoContent();
});

// list all the comments of a given post id
app.MapGet("/posts/{postId}/comments", ([FromServices] PostRepository repo, Guid postId) =>
{
	var comments = repo.FirstOrDefault(x => x.Id == postId)?.Comments;

	return comments != null && comments.Any()
		? Results.Ok(comments)
		: Results.NotFound();
});

// create a comment
app.MapPost("/posts/{postId}/comments", ([FromServices] PostRepository repo, Guid postId, CommentModel model) =>
{
	var post = repo.FirstOrDefault(x => x.Id == postId);

	if (post == null)
		return Results.NotFound();

	Comment commentToSave = new(Guid.NewGuid(), DateTime.Now, model.Author, model.Content, 0);
	post.Comments.Add(commentToSave);

	return Results.Created($"/posts/{postId}/comments", commentToSave);
});

// patch a comment by voting up on given comment id
app.MapMethods("/comments/{id}/vote-up", new[] { "patch" }, ([FromServices] PostRepository repo, Guid id) =>
{
	foreach (var post in repo)
	{
		var comment = post.Comments.FirstOrDefault(x => x.Id == id);
		if (comment != null)
		{
			var index = post.Comments.IndexOf(comment);
			Comment commentToSave = new(id, comment.Date, comment.Author, comment.Content, comment.Votes + 1);
			post.Comments[index] = commentToSave;

			return Results.Ok(commentToSave);
		}
	}

	return Results.NotFound();
});

// patch a comment by voting down on given comment id
app.MapMethods("/comments/{id}/vote-down", new[] { "patch" }, ([FromServices] PostRepository repo, Guid id) =>
{
	foreach (var post in repo)
	{
		var comment = post.Comments.FirstOrDefault(x => x.Id == id);
		if (comment != null)
		{
			var index = post.Comments.IndexOf(comment);
			Comment commentToSave = new(id, comment.Date, comment.Author, comment.Content, comment.Votes - 1);
			post.Comments[index] = commentToSave;

			return Results.Ok(commentToSave);
		}
	}

	return Results.NotFound();
});

// delete a comment
app.MapDelete("/comments/{id}", ([FromServices] PostRepository repo, Guid id) =>
{
	foreach (var post in repo)
	{
		var comment = post.Comments.FirstOrDefault(x => x.Id == id);
		if (comment != null)
		{
			post.Comments.Remove(comment);
			break;
		}
	}

	return Results.NoContent();
});

app.Run();

// ---------------------------------------------------------------------------------------
// Input Models
// ---------------------------------------------------------------------------------------

record PostModel(string Title, string Author, string Content);

record CommentModel(string Author, string Content);

// ---------------------------------------------------------------------------------------
// Models
// ---------------------------------------------------------------------------------------

abstract record BaseModel(Guid Id, DateTime Date);

record Post(Guid Id, string Title, DateTime Date, string Author, string Content, List<Comment> Comments)
	: BaseModel(Id, Date);

record Comment(Guid Id, DateTime Date, string Author, string Content, int Votes)
	: BaseModel(Id, Date);


// ---------------------------------------------------------------------------------------
// Infrastructure
// ---------------------------------------------------------------------------------------

class PostRepository : List<Post> { }
