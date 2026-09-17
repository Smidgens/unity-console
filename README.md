# NOTE

Project has been moved here: https://github.com/Smidgenomics/unity-console

![](/.github/banner.png?raw=true "")
![](/.github/gallery.png?raw=true "")


# ℹ️ Features

* Lightweight developer console for runtime use.
* Minimal UI built with UI Toolkit.
* Supports most primitive types as well as common ones such as `Vector3` or `Color`.
* Versatile debug log.
* Up to 63 custom bit flags used for filtering displayed logs in UI.

## 🧩 Dependencies

* Unity 2023.3
* UI Toolkit

## 🎯 1.0 Goals

* Support methods with optional arguments.
* Support custom input parsing and types.

<br/>

# 🫶 Support

💬 This project is an aggregate of features from at least two console solutions I've written over the years for Unity projects. </br> If you find this plugin useful you can support me by donating, which allows me to allot more of my free time to maintenance (and is very appreciated).


* [Buy Me a Coffee](https://buymeacoffee.com/smidgens)

# 📦 Install

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-console.git#<version_tag>`


<br/>

# 🚀 Overview

## Console Asset

`Create->Console->Console Asset`

An instance of a Console Asset houses all state and command bindings.

<br/>

## Commands

There are three routes for registering commands:

* Script
* Asset
* C# attributes (static commands)

Supported argument types:
* `string` `"example"`
* `float` `10.0`|`10f`
* `int` `1`
* `bool` `t`|`f`
* `Vector2` `<1,2>`
* `Vector3` `<1,2,3>`
* `Vector4` `<1,2,3,4>`
* `Color` `#fff`

### Script

`Component->Smidgenomics->Console->Console Command`

<!-- TODO: Screenshot -->

Scene commands allow fields and methods on scene objects to be registered with Console.

A typical example might be to make the main camera's fov editable through the console.

<br/>


### Asset

`Create->Console->Console Command`.

<!-- TODO: Screenshot -->

Asset commands work the same as scene commands but will persist through loads. These types of commands are useful if you need to bind methods or properties on custom scriptable objects to the console. Exposing player data for example.

To register an asset command with the console it must be added to a Console Asset.


<br/>

### C# Attributes

<!-- TODO: Code examples -->

Attributes are the primary way of registering static fields and methods with console.

Basic setup:
1. Add assembly reference to `Smidgenomics.Unity.Console`.
2. Add `ConsoleAssembly` attribute to your target assembly:</br>`[assembly:ConsoleAssembly]`
3. Add `ConsoleClass` attribute to class whose members you want registered.
4. Add `ConsoleCommand` attribute to field, method, or property.

Notes:
* `ConsoleClass.exposeAll` can be used to register all of its eligible fields and methods with console. This serves as an alternative to `ConsoleCommand`. `HideInConsole` can be used to opt-out specific members out.
* `ConsoleClass.scopeName`, if provided will nest all of its commands under given prefix. For example `settings.resolution`

<br/>

## GUI

Console Window UI can be added to toolkit UI through the UI builder (`Custom Controls/Console/ConsoleWindow` or referenced like so `<Smidgenomics.Unity.Console.ConsoleWindow console-asset="..."/>`. A reference to a Console Asset should be supplied.

### Custom Styling

Several USS variables can be declared in your stylesheet to override the default console style. This will likely be necessary as the defaults are built with 1920x1080 as a reference resolution and will need to be adjusted accordingly for anything else.

Variables:

* `--sm-console-font`
* `--sm-console-spacing`
* `--sm-console-divider-color`
* `--sm-console-background-color`
* `--sm-console-input-height`
* `--sm-console-input-spacing`
* `--sm-console-input-font-size`
* `--sm-console-toolbar-spacing`
* `--sm-console-toolbar-height`
* `--sm-console-focus-color`
* `--sm-console-log-font-size`
* `--sm-console-rounding`
