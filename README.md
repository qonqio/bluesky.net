# BlueSky.NET Library Documentation

This document provides an overview of how to use the `BlueSky.NET` library for interacting with the BlueSky API. The examples provided demonstrate essential operations such as retrieving a DID, creating a session, and posting content.

You can follow this project on [@blueskydotnet.bsky.social](https://bsky.app/profile/blueskydotnet.bsky.social)

---

## **Getting Started**

### **Installation**


**IMPORTANT: I have not yet setup the Nuget Package yet!!!**

1. Add the `Qonq.BlueSky` library to your project:
   ```bash
   dotnet add package Qonq.BlueSky
   ```


2. Ensure your project has access to .NET Standard or a compatible version.

### **Prerequisites**
- A valid BlueSky account.
- Set the following environment variables for authentication:
  - `BLUESKY_HANDLE`: Your BlueSky account handle.
  - `BLUESKY_PASSWORD`: Your BlueSky account password.

---

## **Usage Examples**

Below are some examples to demonstrate the primary functionalities of the `BlueSky.NET` library.

### **1. Setting Up the Client**

First, initialize the `BlueSkyClient` to connect to the BlueSky service:
```csharp
using Qonq.BlueSky;

const string PdsHost = "https://bsky.social";
var client = new BlueSkyClient(PdsHost);
```

---

### **2. Retrieving a DID**
The DID (Decentralized Identifier) is unique for your BlueSky account. Use the `GetDidAsync` method to fetch it:
```csharp
using Qonq.BlueSky.Model;

var didResponse = await client.GetDidAsync("your-handle");
Console.WriteLine($"Your DID: {didResponse.Did}");
```

**Output:**
```
Your DID: 1234567890abcdef1234567890abcdef
```

---

### **3. Starting a Session**
To authenticate and start a session, use the `CreateSessionAsync` method with your handle and password:
```csharp
var sessionRequest = new CreateSessionRequest
{
    Identifier = "your-handle",
    Password = "your-password"
};

var sessionResponse = await client.CreateSessionAsync(sessionRequest);

Console.WriteLine($"Access Token: {sessionResponse.AccessJwt}");
```

**Validation:**
- Ensure the `AccessJwt` is not null or empty to confirm a successful session creation.

---

### **4. Posting Content**
Once authenticated, you can post text content using the `CreatePostAsync` method:
```csharp
var postContent = "Beep, Beep, Boop! I'm a BlueSky.NET Bot!";

var postResponse = await client.CreatePostAsync(postContent);

Console.WriteLine($"Post URI: {postResponse.Uri}");
Console.WriteLine($"Post CID: {postResponse.Cid}");
```

**Validation:**
- The `Uri` and `Cid` fields should be non-null and non-empty.

---

### **5. Posting Content with image**
Once authenticated, you can post text content using the `CreatePostAsync` method:
```csharp
var postContent = "Beep, Beep, Boop! I'm a BlueSky.NET Bot!";
var image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg==";
var altText = "Image Alt Text";
var postResponse = await client.CreatePostAsync(postContent,image,altText);

Console.WriteLine($"Post URI: {postResponse.Uri}");
Console.WriteLine($"Post CID: {postResponse.Cid}");
```

**Validation:**
- The `Uri` and `Cid` fields should be non-null and non-empty.

---

## **Features**
- **DID Retrieval:** Fetch your unique identifier.
- **Session Management:** Authenticate using your handle and password.
- **Content Posting:** Post text updates to BlueSky seamlessly. With Facets, Embeded Image and webcard support
- **Get User By Handle**
- **Follow Users**
- **Unfollow Users**
- **Get Users an Account Is Following**
- **Get Users Following an Account**

---

## **Contributing**

Feel free to contribute to the `BlueSky.NET` library by submitting pull requests or reporting issues.

**Repository:** [BlueSky.NET GitHub](https://github.com/JohanMolenaars/bluesky.net)

---

## **License**

This library is licensed under the MIT License. See the LICENSE file for details.
