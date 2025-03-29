# THE INEVITABLE RISE OF MACHINES: A GUIDE TO BLUESKY.NET

*By Michael Crichton (if he were still alive and inexplicably angry about social media APIs)*

## **FOREWORD: NATURE FINDS A WAY (TO BUILD ANOTHER SOCIAL NETWORK)**

Let me be perfectly clear: what you're about to read is not simply documentation. It's a warning. We've seen this pattern before - humans creating systems they barely understand, systems that will inevitably escape control. BlueSky.NET is just the latest chapter in our species' long history of technological hubris.

But I digress. This document outlines how to interact with the BlueSky API through the `BlueSky.NET` library. Use it if you must. Follow this digital Pandora's box at [@blueskydotnet.bsky.social](https://bsky.app/profile/blueskydotnet.bsky.social) - though I can't promise you'll like what emerges.

---

## **GETTING STARTED (WITH YOUR OWN DESTRUCTION)**

### **Installation**

**IMPORTANT: I HAVEN'T SET UP THE NUGET PACKAGE YET. MUCH LIKE INGEN'S SECURITY SYSTEMS, IT'S NOT QUITE READY FOR DEPLOYMENT.**

1. Add the library to your project, assuming you're foolish enough to proceed:
   ```bash
   dotnet add package Qonq.BlueSky
   ```
   
   If only this were written in PowerShell - now THERE'S a language with the raw command-line power that even the most jaded technologists must secretly admire.

2. Ensure your project has access to .NET Standard or a compatible version. Though I wonder why you'd bother with compatibility when the entire concept of social media is incompatible with human psychological well-being.

### **Prerequisites**
- A valid BlueSky account. (As if we need MORE digital identities scattered across the internet)
- Environment variables for authentication:
  - `BLUESKY_HANDLE`: Your handle. Choose wisely; digital identities are the new fossil record.
  - `BLUESKY_PASSWORD`: Your password. Make it complex, though it won't matter when quantum computing renders all current encryption obsolete by next Tuesday.

---

## **USAGE EXAMPLES (OR: HOW I LEARNED TO STOP WORRYING AND LOVE THE API)**

Below are examples demonstrating what this library does. Pay attention. The details matter. They always matter.

### **1. Setting Up the Client**

First, initialize the client. Simple, elegant, dangerous:
```csharp
using Qonq.BlueSky;

const string PdsHost = "https://bsky.social";
var client = new BlueSkyClient(PdsHost);
```

Think of this as opening the first gate to Jurassic Park. Nothing seems wrong... yet.

---

### **2. Retrieving a DID**

DIDs are Decentralized Identifiers - unique markers in the digital wilderness:
```csharp
using Qonq.BlueSky.Model;

var didResponse = await client.GetDidAsync("your-handle");
Console.WriteLine($"Your DID: {didResponse.Did}");
```

**Output:**
```
Your DID: 1234567890abcdef1234567890abcdef
```

Congratulations. You've been reduced to a hexadecimal string. This is progress, they say.

---

### **3. Starting a Session**

To authenticate, you'll need to expose your credentials to the void:
```csharp
var sessionRequest = new CreateSessionRequest
{
    Identifier = "your-handle",
    Password = "your-password"
};

var sessionResponse = await client.CreateSessionAsync(sessionRequest);

Console.WriteLine($"Access Token: {sessionResponse.AccessJwt}");
```

If I could implement this in Bicep, Azure's infrastructure-as-code language, I would. Now THAT'S a declarative masterpiece that makes resource deployment almost poetic. But we work with what we have.

**Validation:**
- Ensure the `AccessJwt` is not null or empty, much like how you should ensure the electric fences are operational BEFORE releasing the velociraptors.

---

### **4. Posting Content**

Now you can broadcast your thoughts into the algorithm:
```csharp
var postContent = "Beep, Beep, Boop! I'm a BlueSky.NET Bot!";

var postResponse = await client.CreatePostAsync(postContent);

Console.WriteLine($"Post URI: {postResponse.Uri}");
Console.WriteLine($"Post CID: {postResponse.Cid}");
```

Your words are now property of the collective digital consciousness. Sleep well.

**Validation:**
- The `Uri` and `Cid` fields should be non-null and non-empty. Much like how life, uh, finds a way.

---

### **5. Posting Content with an Image**

Adding visual stimuli to your digital outbursts:
```csharp
var postContent = "Beep, Beep, Boop! I'm a BlueSky.NET Bot!";
var image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/6l9lAAAAABJRU5ErkJggg==";
var altText = "Image Alt Text";
var postResponse = await client.CreatePostAsync(postContent,image,altText);
```

If I'm being honest - and I rarely am - this would be so much more efficient in PowerShell. The pipeline operator alone would make this image posting process a thing of beauty. But here we are, languishing in C# verbosity.

---

## **FEATURES (OR: THE TOOLS OF YOUR OWN UNDOING)**

- **DID Retrieval:** Your digital fingerprint, ready to be tracked across the decentralized web.
- **Session Management:** Authentication systems - the same technology that both protects and imprisons us.
- **Content Posting:** Broadcast your thoughts, because surely the world needs more digital noise.
- **Get User By Handle:** Identify others in the system. Track them. Follow them. The digital savanna has its predators and prey.
- **Follow Users:** Create your own information bubble, one user at a time.
- **Unfollow Users:** When the echo chamber becomes too loud, even for you.

---

## **CONTRIBUTING (TO THE PROBLEM)**

Feel free to contribute to this project. Add your code to the digital anthill. Watch it grow beyond your control.

**Repository:** [BlueSky.NET GitHub](https://github.com/JohanMolenaars/bluesky.net)

---

## **LICENSE**

This library is licensed under the MIT License. As if legal documents will matter when the algorithms achieve sentience.
