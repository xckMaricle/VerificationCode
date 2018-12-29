// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


(function ($) {
    $.extend({
        Slider: function (option) {
            var defaultConfig = {
                box: "#box",
                bgColor: ".bgColor",
                txt: ".txt",
                slider: ".slider",
                icon: ".slider>i",
                successText: "验证通过",
                width: "300px",
                height: "250px",
                checkCode: [],
                checkCount: 0,
                selectCount: 0,
                getCodeUrl: '',
                checkCodeUrl: '',
                containerSelector: '',
                successCallback: function (codeList) { }
            };
            $.extend(defaultConfig, option)
            
            if (!defaultConfig.getCodeUrl || !defaultConfig.checkCodeUrl || !defaultConfig.containerSelector) {
                throw new Error("getCodeUrl,checkCodeUrl,containerSelector 不能为空");
            }
            $(defaultConfig.containerSelector).html('');
            $(defaultConfig.containerSelector).append('<div id="box" onselectstart="return false;"><div class="bgColor"></div><div class="txt" id="labelTip">拖动滑块验证</div><div class="slider"><i class="iconfont icon-double-right">>></i></div></div>');
            $(defaultConfig.containerSelector).append('<div id="huadongImage" style="width:' + defaultConfig.width + '; border:1px solid #ccc;height:' + defaultConfig.height +'; z-index:200; display:none; position: absolute;background-color: white;top:40px;"></div>');
            function pointClick(obj) {
                //
            }



            function checkVerificationCode() {

                $.ajax({
                    "url": defaultConfig.checkCodeUrl,
                    "type": "post",
                    "data": {
                        "code": JSON.stringify({ CodeList: defaultConfig.checkCode })
                    },
                    "success": function (d) {
                        if (d.Status == true) {
                            $(defaultConfig.containerSelector).find("#labelTip").html(d.Message);
                            $(defaultConfig.containerSelector).find("#huadongImage").hide();
                            defaultConfig.successCallback(defaultConfig.checkCode);
                        } else {
                            getCodeImage();
                        }
                    },
                    "error": function (error) {

                    }
                })

            }

            function createPoint(pos) {
                if (defaultConfig.selectCount == defaultConfig.checkCount && defaultConfig.checkCodeUrl) {
                    checkVerificationCode();
                    return;
                }
                $(defaultConfig.containerSelector).find("#imagediv").append('<div class="point-area" onclick="pointClick(this)" style="background-color:#539ffe;color:#fff;z-index:9999;width:25px;height:25px;text-align:center;line-height:25px;border-radius: 20%;position:absolute;border:2px solid white;top:' + parseInt(pos.y - 10) + 'px;left:' + parseInt(pos.x - 10) + 'px;">' + defaultConfig.selectCount + '</div>');
                defaultConfig.selectCount += 1;
            }

            function getMousePos(obj, event) {
                var e = event || window.event;
                var x = e.clientX - ($(obj).offset().left - $(window).scrollLeft());
                var y = e.clientY - ($(obj).offset().top - $(window).scrollTop());
                defaultConfig.checkCode.push({ "X": parseInt(x), "Y": parseInt(y) });
                return { 'x': parseInt(x), 'y': parseInt(y) };
            }

            function imageClick() {
                $(defaultConfig.containerSelector).find("#imagediv").click(function () {
                    var _this = $(this);
                    var pos = getMousePos(_this);
                    createPoint(pos);
                })
            }

            function divrefreshClick() {
                $(defaultConfig.containerSelector).find("#divrefresh").click(function () {
                    getCodeImage();
                    defaultConfig.selectCount = 1;
                    defaultConfig.checkCode = [];
                })
            }

            function getCodeImage() {
                $.ajax({
                    url: defaultConfig.getCodeUrl,
                    type: "get",
                    success: function (data) {
                        var html = "<div id=\"imagediv\" style='position: absolute;left:10px; top:30px;background: #fff;z-index:300'><img src=" + data.Result + " alt=\"看不清？点击更换\" id=\"image\"/></div>";
                        html += "<div id='divrefresh' style='width:20px;height:20px;position:absolute;cursor: pointer;margin-left: 90%;'> <img src=\"/images/shaxin.jpg\" /> </div>";
                        $(defaultConfig.containerSelector).find("#huadongImage").css("display", "block").html(html);
                        $(defaultConfig.containerSelector).find("#labelTip").html(data.Message);
                        defaultConfig.checkCount = data.Count;
                        defaultConfig.selectCount = 1;
                        defaultConfig.checkCode = [];
                        imageClick();
                        divrefreshClick();
                    }
                })
            }




            //二、获取到需要用到的DOM元素
            var box = $(defaultConfig.box)[0],//容器
                bgColor = $(defaultConfig.bgColor)[0],//背景色
                txt = $(defaultConfig.txt)[0],//文本
                slider = $(defaultConfig.slider)[0],//滑块
                icon = $(defaultConfig.icon)[0],
                successMoveDistance = box.offsetWidth - slider.offsetWidth,//解锁需要滑动的距离
                downX,//用于存放鼠标按下时的位置
                isSuccess = false;//是否解锁成功的标志，默认不成功

            //三、给滑块添加鼠标按下事件
            slider.onmousedown = mousedownHandler;

            //3.1鼠标按下事件的方法实现
            function mousedownHandler(e) {
                bgColor.style.transition = "";
                slider.style.transition = "";
                var e = e || window.event || e.which;
                downX = e.clientX;
                //在鼠标按下时，分别给鼠标添加移动和松开事件
                document.onmousemove = mousemoveHandler;
                document.onmouseup = mouseupHandler;
            };

            //四、定义一个获取鼠标当前需要移动多少距离的方法
            function getOffsetX(offset, min, max) {
                if (offset < min) {
                    offset = min;
                } else if (offset > max) {
                    offset = max;
                }
                return offset;
            }

            //3.1.1鼠标移动事件的方法实现
            function mousemoveHandler(e) {
                var e = e || window.event || e.which;
                var moveX = e.clientX;
                var offsetX = getOffsetX(moveX - downX, 0, successMoveDistance);
                bgColor.style.width = offsetX + "px";
                slider.style.left = offsetX + "px";

                if (offsetX == successMoveDistance) {
                    success();
                }
                //如果不设置滑块滑动时会出现问题（目前还不知道为什么）
                e.preventDefault();
            };

            //3.1.2鼠标松开事件的方法实现
            function mouseupHandler(e) {
                if (!isSuccess) {
                    bgColor.style.width = 0 + "px";
                    slider.style.left = 0 + "px";
                    bgColor.style.transition = "width 0.8s linear";
                    slider.style.transition = "left 0.8s linear";
                }
                document.onmousemove = null;
                document.onmouseup = null;
            };

            //五、定义一个滑块解锁成功的方法
            function success() {
                isSuccess = true;
                txt.innerHTML = defaultConfig.successText;
                bgColor.style.backgroundColor = "lightgreen";
                slider.className = "slider active";
                icon.className = "iconfont icon-xuanzhong";
                //滑动成功时，移除鼠标按下事件和鼠标移动事件
                slider.onmousedown = null;
                document.onmousemove = null;
                getCodeImage();
            };
        }
    });
})($ || window)