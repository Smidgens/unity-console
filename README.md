![](/.github/banner.png?raw=true "")
![](/.github/gallery.png?raw=true "")


# ℹ️ Features

* Reflection-based in-game console.
* No extras required - usable with existing code and wireable through inspector.
* Bind commands to methods, fields, or properties.
* Customizable toolbar.
* Includes input and parsing of standard primitive types + common Unity structs (`Vector2/3/4`, `Color`...).
* Minimalistic and functional UI (Unity IMGUI).
* 🤞 Reasonably lightweight.
<br/>


**⌛ In Progress**

* [ ] Support binding methods with optional parameters.
* [ ] Support implicit type casting for arguments when available (float -> double etc.)
* [ ] Support custom input parsers to handle custom types as command arguments.

<br/>

# 📦 Install

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-console.git#<version_tag>`


<br/>

# 🚀 Use

## Basic Setup

1. Create a Console asset: `Create->Console->Console`.
2. Add a `Console GUI` script to your scene and drop a reference to the asset into it.

<br/>
<br/>


## Binding Commands

### 🟣 Option 1: Script

1. Add a Console Command script to your scene.
2. Add a reference to a console asset.

<br/>


### 🔵 Option 2: Asset

1. Create a Console Command asset in your project: `Create->Console->Console Command`.
2. Drop a reference to the command into a console asset.

<br/>

### 🟠 Option 3: Attributes

**Note**: Requires reference to `Smidgenomics.Unity.Console` assembly.


#### 🔳 ConsoleCommand

Exposes static method, field, or property to console.


#### 🔳 ConsoleAssembly

Additional options for commands declared in assembly. Also used to filter assemblies based on attribute search settings in console asset.


#### 🔳 ConsoleClass

Additional options for commands declared in type. Includes option for exposing all members automatically.

#### 🔳 HideInConsole

Ignores class member from console when exposing all class members through `ConsoleClass`.


