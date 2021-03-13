# EdgeFramework使用教程

###  一、表格数据
Excels表格数据路径EdgeFramework\Excels\xlsx\，表格数据后缀必须是xlsx，excels表格数据的前四行用于结构定义, 其余为数据

```
第一行：'-' (需要序列化使用的数据)， 'ignore'(代表忽略该列)
第二行：布尔(bool) 整型(int) 浮点数(float) 字符串(string) 数组(array<基本类型>) 枚举(xxxEnum, 自定义名字+Enum后缀)
第三行：关键字(MoveSpeed, 首字母大写式驼峰命名规则)
第四行：注释
```


![image.png](https://upload-images.jianshu.io/upload_images/3912830-8e5756ae716466dc.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

编辑完成表格后必须关闭表格才能转表
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

![image](https://user-images.githubusercontent.com/24520716/110448206-f033b800-80fb-11eb-9a26-d062b8610841.png)

其中有一份配置文件Patch.xml，可打开看到，Name代表要更新的Ab包的包名，url代表更新下载的url,后面是平台，md5和资源的大小

![image](https://user-images.githubusercontent.com/24520716/110448097-d7c39d80-80fb-11eb-9b3f-8999bfe64b16.png)

3.配置Serverinfo.xml服务器配置文件

将工程根节点下/Hot/平台/下的AB包拷贝，然后可在资源服务器下创建文件夹，文件夹路径为   资源服务器/AssetBundle/App版本号/热更版本号/  然后将之前所拷贝出来的热更ab包放入该目录下

随后在资源服务器的根目录下创建Serverinfo.xml,文件内容如下

![image](https://user-images.githubusercontent.com/24520716/110448348-15c0c180-80fc-11eb-81d2-427f08ec00de.png)

GameVersion  Version=为app版本，下面Path为之前打热更包生成出来的Pathces，将拷贝过来，将xmlns移除，并可添加Des更新描述，如上图，下面可添加多个Pathces，但是APP只会对最后一个Pathces进行更新检查，只需要更改服务器端的Pathces即可进行版本的回退以及对版本的更新

服务器路径节点为下图

![image](https://user-images.githubusercontent.com/24520716/110448434-2b35eb80-80fc-11eb-98dc-67b05d3746e0.png)

完成上方操作后运行服务器，并运行APP后可看到已可以更新资源

![image](https://user-images.githubusercontent.com/24520716/110559064-83153680-817e-11eb-81fb-88232b87820d.png)

![image](https://user-images.githubusercontent.com/24520716/110559894-fd928600-817f-11eb-9a46-0d32dc3bbc9d.png)

### 四、BuildAPP
打包Apk的时候可直接点击EdgeFramework->AssetsBundle->BuildApp或者快捷键F12即可启动打包,可看到下图，打包App时会自动设置版本号，自动设置keystore，并重新打Ab包,拷贝入streamassets下。并打打包完毕后自动移除streamassets下的ab包避免git推送
![image](https://user-images.githubusercontent.com/24520716/110449026-ba430380-80fc-11eb-9486-8b1c0671ddd1.png)

### 五、代码规范
 (1)枚举类型和枚举常量都使用大驼峰命名，可加Type后缀
        public enum ExampleType
        {
        None,
        ExampleOne
        }
        
(2)类名一般用大驼峰，即首字母大写,一般我会以对象名相同的名创建
  
(3)类名称尽量少用或不用缩写，若使用了缩写一定要在注释中详细注明类的用途
   
(4)类名要用名词。模板类开头用T。例如TSubject
      
(5)接口开头用I。接口名要用名词。

(6)缩写
        /**
        * GameObject->Go
        * Transform->Trans
        * Position->Pos
        * Button->Btn
        * Dictionary->Dict
        * Number->Num
        * Current->Cur
        * Controller->Ctrl
        */
        
(7)
        /// 私有最好也别省略private
        /// 私有变量可以加前缀 m 表示私有成员 mExampleBtn
        
(8)
        ///公有变量和公共属性使用首字母大写驼峰式类的,尽量用属性代替公有变量。
        
 (9)
        ///常量所有单词大写，多个单词之间用下划线隔开
        
(10)
        ///方法名一律使用首字母大写驼峰式
        
(11)
        ///局部变量最好用var匿名声明 小写驼峰式
        
(12)
        ///静态变量可以加前缀 s 表示静态 sExample
