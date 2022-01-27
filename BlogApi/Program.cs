var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PostRepository>(new PostRepository()
{
	new Post(Guid.NewGuid(), 
		"What's new in .NET 6?", 
		new DateTime(2021, 6, 1), 
		"Bill", 
		"Lorem Ipsum is simply dummy text of the printing and typesetting industry.", 
		new List<Comment>() 
		{ 
			new Comment(Guid.NewGuid(), new DateTime(2021, 7, 8), "Gordon", "Nice job", 4), 
			new Comment(Guid.NewGuid(), new DateTime(2021, 9, 12), "Anne", "Good results.", 2), 
			new Comment(Guid.NewGuid(), new DateTime(2022, 1, 16), "Paul", "Seems to be great", 3) 
		}),
	new Post(Guid.NewGuid(), 
		"By your powers combined I am Captain Planet!", 
		new(2021, 6, 15), "John", "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.", 
		new List<Comment>() 
		{ 
			new Comment(Guid.NewGuid(), new DateTime(2021, 11, 17), "John", "Fire is the best power.", 5), 
			new Comment(Guid.NewGuid(), new DateTime(2021, 12, 21), "Mary", "Wind is great", 1) 
		}),
	new Post(Guid.NewGuid(), 
		"Thunder Cats will save the world!", 
		new(1995, 4, 20), 
		"Lion", "It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.",
		new List<Comment>())
});

var app = builder.Build();

// GET: health check
app.MapGet("/", () => Results.Ok(new { ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id }));

// GET: list the head of all the posts
app.MapGet("/post", (PostRepository repo)
	=> repo.Select(x => new 
	{ 
		x.Id, 
		x.Title, 
		x.Date, 
		x.Author, 
		Uri = $"/posts/{x.Id}" /*HATEOAS?*/
	}).ToList());

// get a post by given id
app.MapGet("/posts/{id}", (PostRepository repo, Guid id) =>
{
	Post? post = repo.FirstOrDefault(x => x.Id == id);

	return post != null
		? Results.Ok(post)
		: Results.NotFound();
});

// create a new post
app.MapPost("/posts", (PostRepository repo, PostModel model) =>
{
	Post post = new(Guid.NewGuid(), model.Title, DateTime.Now, model.Author, model.Content, new List<Comment>());
	repo.Add(post);

	return Results.Created($"/posts/{post.Id}", post);
});

// patch a post by given id
// No MapPatch method yet ;(
app.MapMethods("/posts/{id}", new[] { "patch" }, (PostRepository repo, Guid id, PostModel model) =>
{
	var post = repo.FirstOrDefault(x => x.Id == id);
	if (post == null)
		return Results.NotFound();

	int index = repo.IndexOf(post);
	repo[index] = new(id, model.Title, post.Date, model.Author, model.Content, post.Comments);

	return Results.Ok(post);
});

// delete a post gy given id
app.MapDelete("/posts/{id}", (PostRepository repo, Guid id) =>
{
	repo.RemoveAll(x => x.Id == id);
	return Results.NoContent();
});

// list all the comments of a given post id
app.MapGet("/posts/{postId}/comments", (PostRepository repo, Guid postId) =>
{
	var comments = repo.FirstOrDefault(x => x.Id == postId)?.Comments;

	return comments != null && comments.Any()
		? Results.Ok(comments)
		: Results.NotFound();
});

// create a comment
app.MapPost("/posts/{postId}/comments", (PostRepository repo, Guid postId, CommentModel model) =>
{
	var post = repo.FirstOrDefault(x => x.Id == postId);

	if (post == null)
		return Results.NotFound();

	Comment commentToSave = new(Guid.NewGuid(), DateTime.Now, model.Author, model.Content, 0);
	post.Comments.Add(commentToSave);

	return Results.Created($"/posts/{postId}/comments", commentToSave);
});

// patch a comment by voting up on given comment id
app.MapMethods("/comments/{id}/vote-up", new[] { "patch" }, (PostRepository repo, Guid id) =>
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
app.MapMethods("/comments/{id}/vote-down", new[] { "patch" }, (PostRepository repo, Guid id) =>
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
app.MapDelete("/comments/{id}", (PostRepository repo, Guid id) =>
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

abstract record BaseModel(Guid Id);

record Post(Guid Id, string Title, DateTime Date,  string Author, string Content, List<Comment> Comments)
	: BaseModel(Id);

record Comment(Guid Id, DateTime Date, string Author, string Content, int Votes)
	: BaseModel(Id);


// ---------------------------------------------------------------------------------------
// Infrastructure
// ---------------------------------------------------------------------------------------

class PostRepository : List<Post> { }