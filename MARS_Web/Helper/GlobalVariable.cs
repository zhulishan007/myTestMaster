using MarsSerializationHelper.ViewModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MARS_Web.Helper
{
    public static class GlobalVariable
    {
        public static ConcurrentDictionary<string, ConcurrentDictionary<UserViewModal, List<MarsSerializationHelper.ViewModel.ProjectByUser>>> UsersDictionary { get; set; }
    }
}