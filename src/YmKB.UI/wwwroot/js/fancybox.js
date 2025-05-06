import {Fancybox} from "https://cdn.jsdelivr.net/npm/@fancyapps/ui@5.0.36/dist/fancybox/fancybox.esm.js";

/**
 * 预览文件或处理文件下载的函数
 * @param {string} url - 文件的URL地址
 * @param {Array<string>} gallery - 图片集合（可选），用于在图片预览时构建图片画廊
 */
export function filepreview(url, gallery) {
    console.log(url);
    if (url == null) return;
    const fileName = getFileName(url);
    if (isImageUrl(url)) {
        let images = [];
        if (gallery != null && gallery.length > 0) {
            images = gallery.filter(l => isImageUrl(l)).map(x => ({src: x, caption: x.split("/").pop()}));
        } else {
            images = [{src: url, caption: url.split("/").pop()}];
        }
        const fancybox = new Fancybox(images);
    } else if (isPDF(url)) {
        const fancybox = new Fancybox([{ src: url, type: 'pdf', caption: url }]);
    } else {
        const anchorElement = document.createElement('a');
        anchorElement.href = url;
        anchorElement.download = fileName ?? '';
        anchorElement.click();
        anchorElement.remove();
    }
}
/**
 * 判断URL是否为图片链接的函数
 * @param {string} url - 要检查的URL地址
 * @returns {boolean} - 如果是图片链接返回true，否则返回false
 */
function isImageUrl(url) {
    const imageExtensions = /\.(gif|jpe?g|tiff?|png|webp|bmp)$/i;
    return imageExtensions.test(url);
}
/**
 * 判断URL是否为PDF链接的函数
 * @param {string} url - 要检查的URL地址
 * @returns {boolean} - 如果是PDF链接返回true，否则返回false
 */
function isPDF(url) {
    return url.toLowerCase().endsWith('.pdf');
}
/**
 * 从URL中获取文件名的函数
 * @param {string} url - 包含文件名的URL地址
 * @returns {string|null} - 返回文件名，如果URL为空则返回null
 */
function getFileName(url) {
    return url.split('/').pop();
}