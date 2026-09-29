# Rs64.WebGPU

Rs64 = Reina Sakiria + rs64.net

[wgpu](https://github.com/gfx-rs/wgpu) と [Dawn](https://github.com/google/dawn) のバックエンドを選択可能な、 WebGPU のセーフで多分扱いやすいラッパー

__THIS PROJECT IS UNSTABLE!__

まだ v0.x だよってこと

## How to use

`git submodule add https://github.com/ReinaS-64892/Rs64.WebGPU path` などからクローンして頑張ってバイナリビルドしてね、 nuget はめんどくさくて作る気ないから、(というか、 #if でバックエンドを切り替えるから、nuget でその当たりをうまくやる方法がわからぬ)。


### Build

`./src/Rs64.WebGPU.BindingGenerator` で `dotnet run` してね ... !
でないと、 `Rs64.WebGPU.FFI` の内容が足りずコンパイルエラーになるよ

それと、その際に `./lib/webgpu-headers` (submodule) をクローンするようにね！
ないとジェネレーション出来ないからね

それと、後述の .so などの配置も忘れんようにね

### Select Backend

#### wgpu-native

`./lib/wgpu-native` をそれ単体に対して再帰的に(submoduleの)クローンを行った後 `cargo build` してね
そうすれば .so が `./lib/wgpu-native/target/debug/libwgpu_native.so` に生成されて `./src/Rs64.WebGPU.Binary.wgpu-native` が取り込むから

#### Dawn

`./src/Rs64.WebGPU.Binary.Dawn/binary/libwebgpu_dawn.so` って感じのところに置いといてね

バイナリはこの当たりから入手するといいよ

- https://github.com/ReinaS-64892/webgpu-dawn-build/releases
    - ArchLinux で簡単に動くように少しビルド設定をいじった版
- https://github.com/EmilSV/webgpu-dawn-build/releases
    - 上流のほう、私の環境だと必要なもの多すぎるし用意できなかったから動かなかった ... 


## Special thanks

- https://github.com/EmilSV/WebGPUSharp

- https://github.com/gfx-rs/wgpu-native
    - https://github.com/gfx-rs/wgpu
- https://github.com/google/dawn

- https://github.com/webgpu-native/webgpu-headers
