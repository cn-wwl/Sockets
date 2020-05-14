<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="websocket_easy.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>websockt</title>
    
</head>
<body>
    <form id="form1" runat="server">
       <div id="sse">
        <input type="text" id="msg" name="msg" value="" />
        <input type="button" id="send" name="send" value="Send" />
    </div> 
    </form>
    <script src="Scripts/jquery-3.4.1.min.js"></script>
    <script src="Scripts/WebsocketManager.js"></script>
    <script type="text/javascript">
        $(function () {
            $("#send").click(function () {
                SendMessage($("#msg").val());
            })
        });
        
    </script>
</body>
</html>
