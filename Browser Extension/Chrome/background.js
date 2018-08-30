chrome.browserAction.onClicked.addListener(function () {
    chrome.tabs.create({
        url: "https://github.com/AnjoEscuro/KissAnime-Discord-Rich-Presence/releases"
    });
});