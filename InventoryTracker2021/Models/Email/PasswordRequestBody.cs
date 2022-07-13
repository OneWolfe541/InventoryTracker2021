using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryTracker2021.Models.Email
{
    public class PasswordRequestBody
    {
        public string Body { get; set; }

        public PasswordRequestBody()
        {
            Body = @"
                <html><body>
                <table width=""710"" border=""0"" cellspacing=""0"" cellpadding=""0"" style=""border-collapse:separate;border:none;padding:0;margin:0;table-layout:fixed;"">
    <tr style=""height: 1px"">
        <td style=""border:none;padding:0;"" width=""19px;"" />
        <td style=""border:none;padding:0;"" width=""19px;"" />
        <td style=""border:none;padding:0;"" width=""19px;"" />
        <td style=""border:none;padding:0;"" width=""230px;"" />
        <td style=""border:none;padding:0;"" width=""163px;"" />
        <td style=""border:none;padding:0;"" width=""230px;"" />
        <td style=""border:none;padding:0;"" width=""29px;"" />
    </tr>

    <tr style=""height:29px;"">
        <td colspan=""7"" style=""font-size:1px;"" nowrap>&nbsp;</td>
    </tr>

    <tr style=""height:29px;"">
        <td rowspan=""19"" style=""font-size:1px;"" nowrap>&nbsp;</td>
        <td colspan=""6"" style=""text-align:left;"">
            Hello {0},
        </td>
    </tr>

    <tr style=""height:67px;"">
        <td colspan=""6"">You recently requested to reset your password for you AES Storage Inventory account. Click the link bellow to reset it.</td>
    </tr>

    <tr style=""height:19px;"">
        <td colspan=""5"" style=""font-size:1px;"" nowrap>&nbsp;</td>
    </tr>

    <tr style=""height:26px;"">
        <td colspan=""6"" style=""text-align:center;"">
            <a href=""{1}"">Reset your password</a>
        </td>
    </tr>

    <tr style=""height:19px;"">
        <td style=""font-size:1px;"" nowrap>&nbsp;</td>
    </tr>

    <tr style=""height:67px;"">
        <td colspan=""6"">If you did not request a password reset, please ignore this email or contact AES tech support to let us know. The password reset is only valid for the next 30 minutes.</td>
    </tr>

    <tr style=""height:19px;"">
        <td colspan=""6"" style=""font-size:1px;"" nowrap>&nbsp;</td>
    </tr>

    <tr style=""height:19px;"">
        <td colspan=""3"" style=""font-size:1px;"" nowrap>&nbsp;</td>
    </tr>

    <tr style=""height:134px;"">
        <td colspan=""6"" style=""font-size:1px; text-align:left;"" nowrap>
            <img src='cid:{2}' />
        </td>
    </tr>
</table>
</body> 
</html>";
        }
    }
}