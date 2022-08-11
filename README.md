![](/.github/banner.png?raw=true "")
![](/.github/gallery.png?raw=true "")


## ℹ️ Features

* Reflection-based runtime console for Unity projects.
* No extras required - usable with existing code and wireable through inspector.
* Bind handlers for scene objects or project assets.
* Supports input and parsing of all primitive type arguments + common Unity structs (`Vector2/3/4`, `Color`...).
* Supports multiple input forms (`fn(x)`, `fn x`, `variable=x`)
* Minimalistic and functional UI (Unity IMGUI).
* 🤞 Reasonably lightweight.
<br/>


**⌛ In Progress**

* [ ] Static methods and fields.
* [ ] Optional method parameters
* [ ] Automatic type casting for compatible types (float -> double...)
* [ ] Custom input parsers and type handlers.

<br/>

## 📦 Install

1. Open Package Manager
2. Paste git URL (`<github_url>#<tag>`)


<br/>

## 🚀 Use


1. Create a `Console` asset in your project.
2. Add a `Console GUI` script to your scene and drop a reference to the asset into it.
3. Use `Console Handler` script to bind commands to scene objects.


