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


