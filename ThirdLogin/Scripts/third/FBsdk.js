
// 丟給FB的資料
window.fbAsyncInit = function () {
    FB.init({
        appId: '應用程式編號',
        autoLogAppEvents: true,
        xfbml: true,
        version: 'v9.0',
        status: true
    });
};

async function FBLogin_sdk() {
    // 判斷登入狀態
    await FB.getLoginStatus(function (r) {
        if (r.status === "connected") {
            setInfo();
        }
        else {
            // 第一次登入跳出FB登入畫面
            FB.login(function (response) {
                if (response.status === 'connected') {
                    setInfo();
                }
            }, { scope: 'email', return_scopes: true });
        }
    })
}

// 呼叫API取值
function setInfo() {
    FB.api('/me', "GET", { fields: 'id,name,email' },
        function (r) {
            let id = r.id;
            let name = r.name;
            let email = r.email;

            // 變更登入狀態
            document.getElementById("Login_Account").innerHTML = email;
        });
}