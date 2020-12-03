
// FB手動登入==========================================================
function FBLogin_nosdk() { 
    //第一次呼叫API取URL
    let link = 'https://www.facebook.com/v9.0/dialog/oauth?';
    link += 'client_id=' + '應用程式編號';
    link += '&redirect_uri=' + '有效的OAuth重新導向URI';
    link += '&scope=email';
    window.location.href = link;
}

// 依照網址判斷FB導回來的頁面
if (location.href.includes("code=") && location.href.includes("#_=_")) {
    // 解析要傳送的URL
    var Url = location.href.replace('#_=_', '');
    var aa = Url.indexOf('code=');
    var now_url = location.href.substring(0, aa - 1);

    // 第二次呼叫API取token
    Url = Url.substring(aa + 5, Url.length);
    let client_id = '應用程式編號';
    let link = 'https://graph.facebook.com/v9.0/oauth/access_token?';
    link += '&client_id=' + client_id;
    link += '&redirect_uri=' + now_url;
    link += '&client_secret=' + '應用程式密鑰';
    link += '&code=' + Url;

    var token;
    $.ajax({
        url: link,
        async: false,
        success: (response) => {
            token = response.access_token;
        },
        error: function () {
            alert('登入錯誤，error1');
        }
    });


    // 第三次呼叫API傳送token取資料
    let link2 = 'https://graph.facebook.com/me?';
    link2 += 'access_token=' + token;
    link2 += '&fields=id,name,email';

    var id;
    var name;
    var email;
    
    $.ajax({
        url: link2,
        async: false,
        success: (response2) => {
            id = response2.id;
            name = response2.name;
            email = response2.email
        },
        error: function () {
            alert('登入錯誤，error2');
        }
    });

    // 變更登入狀態
    document.getElementById("Login_Account").innerHTML = email;
}