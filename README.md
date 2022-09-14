![](/.github/banner.png?raw=true "")
![](/.github/gallery.png?raw=true "")


# ℹ️ Features

* Reflection-based in-game console.
* No extras required - usable with existing code and wireable through inspector.
* Bind handlers for scene objects or project assets.
* Supports input and parsing of all primitive type arguments + common Unity structs (`Vector2/3/4`, `Color`...).
* Supports multiple input forms (`fn(x)`, `fn x`, `variable=x`)
* Minimalistic and functional UI (Unity IMGUI).
* 🤞 Reasonably lightweight.
<br/>


**⌛ In Progress**

* [ ] Command help hints, displayable in GUI window through help command.
* [ ] Bind static methods and fields to console, globally or per scene.
* [ ] Automatically register commands through Command (static methods) and ConfigVar (static field) attributes.

**Planned**

* [ ] Optional arguments (C# method arguments with default values).
* [ ] Automatic type casting for compatible types (float -> double...).
* [ ] Custom input parsers and type handlers.
* [ ] Automatic binding of static methods and fields that match configured criteria.

<br/>

# 📦 Install

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-console.git#<version_tag>`


<br/>

# 🚀 Use


1. Create a `Console` asset in your project.
2. Add a `Console GUI` script to your scene and drop a reference to the asset into it.
3. Use `Console Handler` script to bind commands to scene objects.


