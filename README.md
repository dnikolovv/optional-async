# Optional.Async

Async extensions for Nils Luck's Optional library.

It allows you to chain `Task<Option<T>>` and `Task<Option<T, TException>>` without having to use `await`.

Since version `5.0`, the `Optional` library will include its own async extensions. However, at this point they don't provide as much flexibility.

> Note: `Optional.Async` was not meant to be published as a standalone library. It's a collection of extension methods that I had to implement on my own while working with `Optional`. There are currently no tests and I don't plan on extensively maintaining it. Still, most of the extensions you'll find here are currently being used/have been used for the past year in production code without issues.

# Usage

---

Say you have the following asynchronous functions and you want to execute them in order:

```csharp
Task<Option<User, Error>> CheckIfUserIsAuthorized(string userId, string category);

Task<Option<CloudRecord, Error>> StoreTheFileIntoTheCloud(File file, string category);

Task<Option<Guid, Error>> StoreDatabaseLog(CloudRecord record);

Task<Option<DocumentProcessedResult, Error>> SendToExternalService(Guid key);
```

Currently, there is no built-in mechanism inside `Optional` that allows you to chain these calls. What `Optional.Async` provides is the ability to compose asynchronous functions. The following would be valid syntax:

```csharp
Task<Option<DocumentProcessedResult, Error>> ProcessDocument(
    string userId,
    string category,
    File file) =>
    CheckIfUserIsAuthorized(userId, category).FlatMapAsync(user =>
    StoreTheFileIntoTheCloud(file, category).FlatMapAsync(cloudRecord =>
    StoreDatabaseLog(cloudRecord).FlatMapAsync(uniqueKey =>
    SendToExternalService(uniqueKey))));
```

See the [DevAdventures RealWorld project](https://github.com/dnikolovv/dev-adventures-realworld) and the [DDD Café](https://github.com/dnikolovv/cafe) for more examples.

# How to install

It's available as a NuGet package under `Optional.Async`.
