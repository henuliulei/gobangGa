using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Service
{
    //需要服务端实现的协定
    [ServiceContract(Namespace = "WcfGobangGameExample",
        SessionMode = SessionMode.Required,
        CallbackContract = typeof(IGobangServiceCallback))]
    public interface IGobangService
    {
        [OperationContract(IsOneWay = true)]
        void Login(string userName);

        [OperationContract(IsOneWay = true)]
        void Logout(string userName);

        [OperationContract(IsOneWay = true)]
        void SitDown(string userName, int index, int side);

        [OperationContract(IsOneWay = true)]
        void GetUp(int index, int side);

        [OperationContract(IsOneWay = true)]
        void Start(string userName, int index, int side);

        [OperationContract(IsOneWay = true)]
        void SetDot(int index, int i, int j,int k);

        [OperationContract(IsOneWay = true)]
        void SetDot_1(int index, int i, int j,int k,int i1,int j1);

        [OperationContract(IsOneWay = true)]
        void Talk(int index, string userName, string message);
    }

    //需要客户端实现的接口
    public interface IGobangServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ShowLogin(string loginUserName, int maxTables);

        [OperationContract(IsOneWay = true)]
        void ShowLogout(string userName);

        [OperationContract(IsOneWay = true)]
        void ShowSitDown(string userName, int side);

        [OperationContract(IsOneWay = true)]
        void ShowGetUp(int side);

        [OperationContract(IsOneWay = true)]
        void ShowStart(int side);

        [OperationContract(IsOneWay = true)]
        void ShowTalk(string userName, string message);

        [OperationContract(IsOneWay = true)]
        void ShowSetDot(int i, int j, int k);

        [OperationContract(IsOneWay = true)]
        void ShowSetDot_1(int i, int j, int k,int i1,int j1);

        [OperationContract(IsOneWay = true)]
        void GameStart();

        [OperationContract(IsOneWay = true)]
        void GameWin(string message);

        //各桌玩家情况，包括那一桌的哪一方有人，以及游戏室目前多少人，客户端接受到此信息后，
        //需要解析tablesInfo字符串，并形象的显示各桌情况
        [OperationContract(IsOneWay = true)]
        void UpdateTablesInfo(string tablesInfo, int userCount);
    }
}
