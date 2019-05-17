(function () { window.BMap_loadScriptTime = (new Date).getTime(); document.write('<script type="text/javascript" src="http://api.map.baidu.com/getscript?v=3.0&ak=gzZm5odUghcAXqgf5KVvxoLXn5hUXWLv&services=&t=20180626110404"></script>'); })();

(function ($) {
    $.extend($.fn, {
        /**
         * @description 地图组件
         * @version 0.1.1-beta
         * @author 李小阳
         */
        baiduMap: function (a, b) {
            var eles = this;
            var options = {
                lng: 118.156801, lat: 24.485408, controllers: true, wheelZoom: true, suggestId: undefined, onSuggestSelect: $.noop, level: 12, cursor: 'default', onClick: $.noop
            }
            if ($.isPlainObject(a) || a === undefined) {
                $(eles).each(function () {
                    $.extend(options, a);
                    options.onClick = options.onClick.bind(this);
                    options.onSuggestSelect = options.onSuggestSelect.bind(this);
                    var map = new BMap.Map(this); //创建地图
                    if (options.wheelZoom) {
                        map.enableScrollWheelZoom();     //开启鼠标滚轮缩放
                    }
                    if (options.suggestId) {
                        var ac = new BMap.Autocomplete(    //建立一个自动完成的对象
                       {
                           "input": $(options.suggestId).get(0),
                           "location": map
                       });

                        ac.addEventListener("onconfirm", function (e) {    //鼠标点击下拉列表后的事件
                            var _value = e.item.value;
                            var myValue = _value.province + _value.city + _value.district + _value.street + _value.business;
                            options.onSuggestSelect(myValue, _value);
                        });

                    }
                    map.centerAndZoom(new BMap.Point(options.lng, options.lat), options.level); //设置中心
                    map.setDefaultCursor(options.cursor); //设置默认鼠标样式
                    map.addEventListener("click", options.onClick); //监听点击
                    if (options.controllers) {
                        var top_left_control = new BMap.ScaleControl({ anchor: BMAP_ANCHOR_TOP_LEFT });// 左上角，添加比例尺
                        var top_left_navigation = new BMap.NavigationControl();  //左上角，添加默认缩放平移控件
                        map.addControl(top_left_control); //添加控件
                        map.addControl(top_left_navigation);//添加控件
                    }
                    var local = new BMap.LocalSearch(map, {
                        renderOptions: { map: map }
                    });
                    var loaded = function () {
                        $('.BMap_cpyCtrl').remove();//移除百度版权说明
                        
                        $('.anchorBL').remove();//移除百度版权说明
                        map.removeEventListener("tilesloaded", loaded);
                    };
                    map.addEventListener("tilesloaded", loaded);
                    $(this).data('map', map);
                    var methods = {
                        center: function (o) {
                            $.extend(options, o);
                            map.centerAndZoom(new BMap.Point(options.lng, options.lat), options.level); //设置中心
                        },
                        clearMarker: function () {
                            map.clearOverlays();
                        },
                        addMarker: function (o) {
                            marker = new BMap.Marker(new BMap.Point(o.lng, o.lat));
                            map.addOverlay(marker);

                        },
                        search: function (value) {
                            local.search(value);
                        },
                        map: function () {
                            return map;
                        },
                        removeMarker: function (point) {
                            var allOverlay = map.getOverlays();
                            for (var i = 0; i < allOverlay.length; i++) {
                                //删除指定经度的点
                                if (allOverlay[i] instanceof BMap.Marker) {
                                    if (allOverlay[i].getPosition().lng == point.lng && allOverlay[i].getPosition().lat == point.lat) {
                                        map.removeOverlay(allOverlay[i]);
                                      
                                    }
                                }

                            }
                        },
                        getSuggessPoint: function (options) {


                            var local = new BMap.LocalSearch(map, { //智能搜索
                                onSearchComplete: function myFun() {
                                    var pp = local.getResults().getPoi(0).point;    //获取第一个智能搜索的结果
                                    options.success(pp);
                                }
                            });
                            local.search(options.address);
                        }

                    }
                    $(this).data('methods', methods);
                })
                return $(this);
            } else {
                var reulst;
                $(eles).each(function () {
                    var map = $(this).data('map');
                    var local = new BMap.LocalSearch(map, {
                        renderOptions: { map: map }
                    });
                    methods = $(this).data('methods');

                    result = methods[a](b);
                })
                if (result) return result;
                return $(this);
            }
        }
    })
})(jQuery);

