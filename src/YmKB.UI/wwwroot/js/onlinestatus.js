// 提供与浏览器在线状态（online/offline）交互的功能，用于前端检测网络连接变化，并通知 .NET 对象更新状态
window.onlineStatusInterop = {
    getOnlineStatus: function () {
        return navigator.onLine;
    },
    addOnlineStatusListener: function (dotNetObjectRef) {
        const onlineHandler = () => {
            dotNetObjectRef.invokeMethodAsync('UpdateOnlineStatus', true);
        };
        const offlineHandler = () => {
            dotNetObjectRef.invokeMethodAsync('UpdateOnlineStatus', false);
        };

        window.addEventListener('online', onlineHandler);
        window.addEventListener('offline', offlineHandler);
    }
};