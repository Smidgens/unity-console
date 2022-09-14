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


**⌛ In Progress **

* [ ] Optional method arguments.
* [ ] Support implicit type casting for arguments when available (float -> double etc.)
* [ ] Support custom input parsers to handle custom types as command arguments.

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


