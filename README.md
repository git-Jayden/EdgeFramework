# EdgeFramework使用教程
### 一、AssetsBundle打包
######  1.AppConfig配置
点击EdgeFramework->Tools->AppConfig或者快捷键F6调出AssetBundle的配置文件

如下图，AllPrefabPath为配置需要打AB包的Prefab的路径，只需要填写至文件夹的路径即可，打包的包名即为Prefab的名字，AllFileDirAb下即填写需要打包的资源的文件夹路径以及填写需要打包生成的AB包名

*注:如Prefab包中包含了某些资源文件则该资源文件可不必打包入资源中，例如UIPrefab中包含了某几张图片，则这些图片不必再打AB包*

![image](https://user-images.githubusercontent.com/24520716/110445896-903c1200-80f9-11eb-8018-baae970dfeb9.png)

###### 2.BuildBundle  打包说明
点击EdgeFramework->AssetsBundle->BuildBundle或者快捷键F8即可启动打包

打包所生成文件

（1）AssetbundleCofig.xml文件 该xml只用来查看打包信息

该文件在Assets根目录下，打开后如下图ABList代表打包的资源，Path代表该打包资源的原资源路径，Crc为该资源路径的唯一ID,ABName代表该资源打入了这个Ab的包名中,AssetName代表资源的名字

另ABDependce代表依赖项，意思就是加载ABList中资源还依赖于该ABDependce Ab包中的资源
![image](https://user-images.githubusercontent.com/24520716/110446267-f1fc7c00-80f9-11eb-9f0f-2d1299b4ba25.png)
注:如遇资源加载报错打开该xml根据报错的Crc看资源路径，xml中如果没有该crc代表资源没打入包中或者资源加载的路径填写错误


（2）AssetbundleConfig.bytes文件 该bytes内容与上面的XML一致，只不过这个文件是用来加载使用，该文件所在目录Assets/ABResources/Data/Config

（3）ABMD5.bytes本地md5校验文件，资源所在目录Assets\Resources下，该文件是用来校验本地本地资源的解压，在程序开始运行的时候会将StreamAssets下的AssetsBundle解压入Application.persistentDataPath持久化数据存储目录中，这时候就需要Md5值校验文件

![image](https://user-images.githubusercontent.com/24520716/110446473-2bcd8280-80fa-11eb-8de1-829561f19269.png)

（4）AssetsBundle包 在工程根目录下AssetBundle\平台\

（5）ABMD5.bytes服务器md5校验文件 在工程根目录下Version\平台\ABMD5_0.1.bytes   0.1为App的版本 打热更包的时候会选择app版本号中的md5做对比，会对比出与最初打包出来Ab包中的资源文件哪些文件做了更改


### 二、AssetsBundle资源加载
1.AssetsBundle加载配置

需要资源加载需将AppConfig.cs脚本中的UseAssetBundle设置为true,并将工程根目录下AssetBundle\平台\下的Ab包拷贝到StreamAssets\AssetBundle\目录下这时候可以使用EdgeFramework->AssetsBundle->CopyBundleToStreamAssets可直接将Ab包拷贝入Stream目录下，如需移除streamAssets下Ab包也可点击EdgeFramework->AssetsBundle->DeleteStreamAssets下自动移除Ab包，建议在Editor下使用编辑器加载。需打Apk的时候使用Ab加载

2.AssetsBundle代码加载

（1）资源同步加载ResourcesManager.LoadResouce(string path)，具体其他函数可自行查看ResourcesManager,包括预加载资源，异步加载资源，资源卸载，取消异步加载资源等

（2）Prefab同步加载ObjectManager.InstantiateObject(string path)，具体其他函数可自行查看ObjectManager,包括预加载Gamobject，异步加载，回收资源，取消异步加载资源等

### 三、打热更包以及热更配置文件的配置
1.配置资源热更

打开AppConfig.cs脚本，将CheckVersionUpdate检查更新设置为true,并且设置好ServerResourceURL资源的url路径

2.打热更包

（1）点击EdgeFramework->AssetsBundle->打热更包，如下图，选择当前app版本所打出来AssetsBundle包生成出来的ABMD5.bytes文件，下面热更补丁版本为热更的版本，代表第几次热更的版本，意思将当前的资源文件与最初的资源文件的md5做对比，如当前资源与之前的资源文件Md5不一致代表该资源需更新重新打一份ab包出来
![image](https://user-images.githubusercontent.com/24520716/110447136-e198d100-80fa-11eb-9e6c-95525f69b957.png)

点击打热更包后，会生成差异的ab文件以及一份热更配置文件出来，目录在工程根节点下/Hot/平台/下，资源目录如下图

![image](https://user-images.githubusercontent.com/24520716/110447946-b2cf2a80-80fb-11eb-9ccd-8f01740ec9bd.png)

其中有一份配置文件Patch.xml，可打开看到，Name代表要更新的Ab包的包名，url代表更新下载的url,后面是平台，md5和资源的大小

![image](https://user-images.githubusercontent.com/24520716/110448097-d7c39d80-80fb-11eb-9b3f-8999bfe64b16.png)
![image](https://user-images.githubusercontent.com/24520716/110447946-b2cf2a80-80fb-11eb-9ccd-8f01740ec9bd.png)


