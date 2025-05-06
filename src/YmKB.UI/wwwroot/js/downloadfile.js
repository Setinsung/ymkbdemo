// 全局函数，用于从流中下载文件
window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    // 将流内容转换为ArrayBuffer
    const arrayBuffer = await contentStreamReference.arrayBuffer();

    // 创建Blob对象，封装二进制数据
    const blob = new Blob([arrayBuffer]);

    // 创建一个指向Blob的临时URL
    const url = URL.createObjectURL(blob);

    // 创建一个<a>元素用于触发下载
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? ''; // 设置下载文件名

    // 模拟点击链接以开始下载
    anchorElement.click();

    // 移除创建的<a>元素
    anchorElement.remove();

    // 释放创建的URL对象，避免内存泄漏
    URL.revokeObjectURL(url);
}