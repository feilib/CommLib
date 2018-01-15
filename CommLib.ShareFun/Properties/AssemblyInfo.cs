using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("CommLib.ShareFun")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("HXJL")]
[assembly: AssemblyProduct("CommLib.ShareFun")]
[assembly: AssemblyCopyright("Copyright © SkyUN.Org 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("5cc70289-41cb-4eb2-b25b-3676fb615c4a")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“内部版本号”和“修订号”的默认值，
// 方法是按如下所示使用“*”:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.1.2.1")]
[assembly: AssemblyFileVersion("1.1.2.1")]

/*----------------------
* 更新历史：
* 2017年8月24日  V1.1.0.1
* 1. 增加了是否管理员权限运行的代码。
* 2. 增加了对服务是否存在的判断。
* 
* 2017年8月29日 V1.1.0.2
* 1. 增加了重名文件的判断以及新名称的生成
* 
* 2017年11月21日 V1.1.0.3
* 1. 增加了CRC32和CRC16的计算算法，还待验证
* 
* 2018年1月12日 V1.1.1.1
* 1. 增加了循环队列的处理逻辑RingBufferManager，便于modbus、tcp等地方调用。
* 2. 对RingBufferManager增加了一些注释
* 
* 2018年1月12日 V1.1.1.2
* 1. 增加了循环队列的单元测试，主要测试性能和锁。
* 2. 测出来问题，发现不加锁容易跑飞，现已经在循环队列中加锁。。。
* 
* 2018年1月13日 V1.1.2.0
* 1. 增加了异步TCP服务端代码，还未测试。。
* 
* 2018年1月15日 V1.1.2.1
* 1. 修正了TCP服务器，委托为空时调用异常的bug。
* 2. tcp收到报文上报的委托也要回传本身的socket object，方便上层调用
* 3. 增加了tcp服务器的单元测试，基本可用。。
------------------------*/

