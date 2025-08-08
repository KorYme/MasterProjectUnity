# MasterProject

#### This unity project is meant to prototype and create packages that can be used in future projects.

## Internal Packages

_To add these packages to your projects, use the link next to its name in the Unity Package Manager with "from git url" option_

`ToolbarExtender` : https://github.com/KorYme/MasterProjectUnity.git?path=/Packages/ToolbarExtender

> Create a scene dropdown in the unity editor dropdown to easily change scene.
> 
> See : https://github.com/marijnz/unity-toolbar-extender for more details (this package is based on his work).

---

`Manager Gameloop Architecture` : https://github.com/KorYme/MasterProjectUnity.git?path=/Packages/ManagerGameloopArchitecture

> Easy to use gameloop architectures with singleton workaround using injection and SOAP.
> 
> Requires :
> 1. `NSubstitute` : https://github.com/Thundernerd/Unity3D-NSubstitute.git

---

`Save System` : https://github.com/KorYme/MasterProjectUnity.git?path=/Packages/SaveSystem

> Add a struct to serialize your scene as object instead of string or integer.
>
> Requires :
> 1. `Encryption Utils` : https://github.com/KorYme/MasterProjectUnity.git?path=/Packages/EncryptionUtils
> 2. `NewtonSoft Json` : "com.unity.nuget.newtonsoft-json"

---

`SceneReference Utils` : https://github.com/KorYme/MasterProjectUnity.git?path=/Packages/SceneReferenceUtils

> Add a struct to serialize your scene as object instead of string or integer.

---

`Encryption Utils` : https://github.com/KorYme/MasterProjectUnity.git?path=/Packages/EncryptionUtils

> Static methods used to encrypt and decrypt data.
> 
> Available Encryption algorithm for the moment :
> 1. XOR