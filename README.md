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
The DID (Decentralized Identifier) is unique for your BlueSky account. Use the `GetDid` method to fetch it:
```csharp
using Qonq.BlueSky.Model;

var didResponse = await client.GetDid("your-handle");
Console.WriteLine($"Your DID: {didResponse.Did}");
```

**Output:**
```
Your DID: 1234567890abcdef1234567890abcdef
```

---

### **3. Starting a Session**
To authenticate and start a session, use the `CreateSession` method with your handle and password:
```csharp
var sessionRequest = new CreateSessionRequest
{
    Identifier = "your-handle",
    Password = "your-password"
};

var sessionResponse = await client.CreateSession(sessionRequest);

Console.WriteLine($"Access Token: {sessionResponse.AccessJwt}");
```

**Validation:**
- Ensure the `AccessJwt` is not null or empty to confirm a successful session creation.

---

### **4. Posting Content**
Once authenticated, you can post text content using the `CreatePost` method:
```csharp
var postContent = "Hello, BlueSky!";

var postResponse = await client.CreatePost(postContent);

Console.WriteLine($"Post URI: {postResponse.Uri}");
Console.WriteLine($"Post CID: {postResponse.Cid}");
```

**Validation:**
- The `Uri` and `Cid` fields should be non-null and non-empty.

---

## **Features**
- **DID Retrieval:** Fetch your unique identifier.
- **Session Management:** Authenticate using your handle and password.
- **Content Posting:** Post text updates to BlueSky seamlessly.

---

## **Contributing**

Feel free to contribute to the `BlueSky.NET` library by submitting pull requests or reporting issues.

**Repository:** [BlueSky.NET GitHub](https://github.com/Qonq/BlueSky.NET)

---

## **License**

This library is licensed under the MIT License. See the LICENSE file for details.