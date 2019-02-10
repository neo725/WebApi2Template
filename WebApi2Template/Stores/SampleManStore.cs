using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi2Template.Models;

namespace WebApi2Template.Stores
{
    public class SampleManStore : BaseStore<SampleManModel>
    {
        // 沒有寫建構式，將會使用 BaseStore 中的常數 _defaultConnectionStringName 所設定的名稱去連線
    }
}