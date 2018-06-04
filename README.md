# fastdfs-client-net
fastdfs的生产环境级别的客户端
感谢之前这种开源客户端的代码实现，借鉴这些代码，才能迅速地实现各个项目

## other
code is at folder:code
fastdfs-client-net 命名参考了余大的命名方式

## 核心问题
    1. 解决tcp连接长时间等待问题
    2. 解决服务端异常后，客户端必须重启问题
    3. 解决多tracker高可用问题
    4. 需要解决断点续传问题（todo）
    5. 访问安全问题（todo）
    6. 集成错误日志
    7. 添加元数据记录日志
    8. 元数据服务集成（待续，不一定实现）
    9. 定义对外统一facade接口
    10. 上传和下载均支持重试机制

## 版本问题
支持服务端5.08 ，其他未测试，估计也没问题

## how to use
add configs to appSetting or web.config
```java
    <appSettings>
        <add key="fastdfs_Nodes" value="192.168.12.7:22122,192.168.12.8"/>
        <add key="fastdfs_GroupName" value="group1"/>
        <add key="fastdfs_DownloadUrl" value="http://192.168.xx.xx:8080/group1/" />
    </appSettings>
```

code like:
```java
    //upload
    string serverFileName = fastdfs_client_net.Facade.Upload("D:/xxxx.xls");
    //download
    fastdfs_client_net.Facade.DownLoad("M00/00/00/wKgMyli04auAdI0FAAASAFMyJ8E647.xls", "D:/xxx/target.xls", true);
```

## 客户端错误日志，及元数据日志
可通过fastdfs_LogPath 配置日志路径，详细见类：FDFSConfig
日志默认路径为程序目录下：FileServerLogs
日志支持归档，采用zip压缩，最大归档大小为50M
日志记录like：
```java
    [时间：2018-06-04 16:14:07级别：INFO 日志内容]：D:/temp.xls|50688||M00/01/39/wKgMB1sU9dqAE0x6AADGAKB9VCU109.xls|UPLOAD
    [时间：2018-06-04 16:14:07级别：INFO 日志内容]：M00/01/39/wKgMB1sU9dqAE0x6AADGAKB9VCU109.xls||第1次下载|E:/temp/t10159.xls|DOWNLOAD
    [时间：2018-06-04 16:14:07级别：INFO 日志内容]：D:/temp.xls|50688||M00/01/5A/wKgMCFsU9dqAK7coAADGAKB9VCU596.xls|UPLOAD
    [时间：2018-06-04 16:14:07级别：INFO 日志内容]：M00/01/5A/wKgMCFsU9dqAK7coAADGAKB9VCU596.xls||第1次下载|E:/temp/t10160.xls|DOWNLOAD
    [时间：2018-06-04 16:14:08级别：INFO 日志内容]：D:/temp.xls|50688||M00/01/39/wKgMB1sU9dqAWaE-AADGAKB9VCU194.xls|UPLOAD
    [时间：2018-06-04 16:14:08级别：INFO 日志内容]：M00/01/39/wKgMB1sU9dqAWaE-AADGAKB9VCU194.xls||第1次下载|E:/temp/t10161.xls|DOWNLOAD
    [时间：2018-06-04 16:14:08级别：INFO 日志内容]：D:/temp.xls|50688||M00/01/5A/wKgMCFsU9dqAWYSDAADGAKB9VCU316.xls|UPLOAD
```