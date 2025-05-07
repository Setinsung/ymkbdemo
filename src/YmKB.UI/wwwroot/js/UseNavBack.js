let dotNetObject;

async function handlePopState() {

    const dontPop = await dotNetObject.invokeMethodAsync("HandlePopState");

    if (dontPop) {
        // add the current location back to stack
        history.pushState({}, "", window.location.href);
    }
    else {
        window.removeEventListener("popstate", handlePopState);
        await history.back();
    }

}

export async function handleUiBackButton() {

    const dontPop = await dotNetObject.invokeMethodAsync("HandlePopState");

    if (!dontPop) {
        window.removeEventListener("popstate", handlePopState);
        // -2 因为将当前位置作为堆栈上的前两个条目
        await history.go(-2);
    }
}

export async function addPopStateListener(dotNetInstance) {
    dotNetObject = dotNetInstance;
    // 将当前位置的额外副本添加到 Stack 中，以便在浏览器 Back （返回） 按钮弹出时不会更改。    await history.pushState({}, "", window.location.href);
    window.addEventListener("popstate", handlePopState);
}

export function removePopStateListener() {
    window.removeEventListener("popstate", handlePopState);
}