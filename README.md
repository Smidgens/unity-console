![](/.github/banner.png?raw=true "")
![](/.github/gallery.png?raw=true "")


# ℹ️ Features

* Reflection-based in-game console.
* No extras required - usable with existing code and wireable through inspector.
* Bind commands to methods, fields, or properties.
* Includes input and parsing of standard primitive types + common Unity structs (`Vector2/3/4`, `Color`...).
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

<br/>

# 📦 Install

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-console.git#<version_tag>`


<br/>

# 🚀 Use


1. Create a `Console` asset in your project.
2. Add a `Console GUI` script to your scene and drop a reference to the asset into it.
3. Use `Console Commands` script to bind commands to scene objects.


