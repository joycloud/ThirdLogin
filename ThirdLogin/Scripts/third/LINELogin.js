// Line登入========================================================
function LineAuth() {

    // Line access抓授權碼
    let client_id = '1655199286';
    let link = 'https://access.line.me/oauth2/v2.1/authorize?';
    let state = location.href;
    link += 'response_type=code';
    link += '&client_id=' + client_id;
    link += '&redirect_uri=' + 'https://localhost:44342/Home/GuideView';
    link += '&state=' + state;
    link += '&scope=openid%20profile%20email';

    // 跳登入頁(若是用LINE瀏覽器開，會直接登入)
    window.location.href = link;
}

