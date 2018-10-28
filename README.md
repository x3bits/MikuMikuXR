# MikuMikuXR

MikuMikuXR is a MikuMikuDance player powered by [libmmd-for-unity](#https://github.com/x3bits/libmmd-for-unity), which supports VR glass, AR camera and holographic pyramid.
This application is developed by unity. It works on Android. It has not been test on other platform yet.

## License

MikuMikuXR is free software available under the 3-Clause BSD License. See the file "LICENSE" for license conditions.

## Build
This software is tested under Unity 3D 2018.2.13f1. Vuforia is needed when installing Unity 3D. 
Since some restrictions of the Vuforia Developer Agreement, files of Vuforia is not published with the source code. It is needed to import Vuforia after opening the project in Unity 3D. Operation is as follows: 
Open the project in Unity 3D. Right click the "Hierachy" window. Choose "Vuforia -> AR Camera". Then follow the tip of Unity 3D to import Vuforia. Don't forget to delete the AR Camera in hierarchy window after importing Vuforia.

## Usage
Install the application on your Android device. Put your mikumikudance model, motion, camera data under the folder "MikuMikuAR" under the storage path of your Android device so that the application can find them.

## 3rd party software

MikuMikuXR makes use of the following 3rd party software:

- libmmd-for-unity - Copyright (c) 2017, x3bits (x3bits@sina.com) - provided under the 3-Clause BSD license.
  https://github.com/x3bits/libmmd-for-unity

- mmd-for-unity - Copyright (c) 2011, Eiichi Takebuchi, Takahiro Inoue, Shota Ozaki, Masamitsu Ishikawa, Kazuki Yasufuku, Fumiya Hirano. - provided under the 3-Clause BSD license.

  https://github.com/mmd-for-unity-proj/mmd-for-unity

- BulletSharpUnity3d - Copyright for portions of project BulletSharp and BulletSharpPInvoke are held by Andres Traks, 2013-2015 as part of project BulletSharpUnity3d - provided under the zlib/libpng License

  https://github.com/Phong13/BulletSharpUnity3d

- MMDCameraPath -  Copyright {@2017} { @Rumeng (chinesename liuzhanhao) } email:liuzhanhao96@126.com Licensed under the Apache License, Version 2.0 (the "License")

  https://github.com/lzh1590/MMDCameraPath

- TTUIFramework
  https://github.com/chiuan/TTUIFramework

- List View Framework - Copyright (c) 2016, Unity Technologies - provided under the MIT license.

  https://bitbucket.org/Unity-Technologies/list-view-framework

- SharpCompress - Copyright (c) 2014  Adam Hathcock - provided under the MIT license.

  https://github.com/adamhathcock/sharpcompress