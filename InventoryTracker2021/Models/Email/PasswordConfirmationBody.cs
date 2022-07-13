using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryTracker2021.Models.Email
{
    public class PasswordConfirmationBody
    {
        public string Body { get; set; }

        public PasswordConfirmationBody()
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
        <td colspan=""6"">Your password on the AES Storage Inventory site was successfully changed.</td>
    </tr>

    <tr style=""height:67px;"">
        <td colspan=""6"">If you have not recently changed your password or received this email by mistake please contact Tech Support.</td>
    </tr>

    <tr style=""height:19px;"">
        <td colspan=""6"" style=""font-size:1px;"" nowrap>&nbsp;</td>
    </tr>

    <tr style=""height:134px;"">
            <td colspan=""6"" style=""font-size:1px; text-align:left;"" >
                <img src='cid:{1}' />
            </td>
        </tr>
</table>
</body> 
</html>";
        }
    }
}