define([
    "dojo/_base/declare",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin"
],
    function (
        declare,
        _Widget,
        _TemplatedMixin,
        _WidgetsInTemplateMixin
    ) {
        return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin], {
            templateString: dojo.cache("/EPiServer/Zone.Epi.ABTestListGadget/ABTestListComponent/"),

            postCreate: function () {
                // Get the data to populate the template
                this.inherited(arguments);
                window.globalPageVar = 0;
                window.globalResultsVar = 0;
                window.globalDropDownPopulated = false;
                window.globalDropDownFilterSite = "All";

                // Only run our call function if the element to inject into is available
                if (this.abtesttable !== null) {
                    this._getAbTests();

                    // Also setup the paging buttons event listeners
                    this._setupButtonClickListeners();
                }
            },

            _getAbTests() {
                var that = this;
                $.get('/EPiServer/Zone.Epi.ABTestListGadget/ABTestListComponent/GetModel?page=' + window.globalPageVar +
                    "&filterOnSitename=" + window.globalDropDownFilterSite,
                    function (data) {
                        if (data === null || data === "") {
                            // No AB tests at all, disable buttons
                            if (window.globalPageVar === 0 && window.globalResultsVar === 0) {
                                that.abtesttable.innerHTML = "No active A/B tests";
                                that.abtestpagenext.classList.add("hidden");
                                that.abtestpageprevious.classList.add("hidden");
                            } else {
                                // We have data, just not for this page
                                that.abtesttable.innerHTML = "No more active A/B tests";
                                that.abtestpagecounter.innerHTML = "Page " + (window.globalPageVar + 1);
                                window.globalResultsVar = 0;

                                that._updatePagingButtonsState();
                            }
                        } else {
                            // We have some data to show, time for some json fun
                            var json = JSON.parse(data);

                            tbl = that.abtesttable;
                            tbl.innerHTML = null;

                            that._addRow(tbl, 'Title', null, 'Started By', 'Start Date', 'End Date', 'Participation', 'A/B Views', 'A/B Conversions');

                            for (var i = 0; i < json.ActiveTestList.length; i++) {
                                that._addRow(tbl,
                                    json.ActiveTestList[i].Title,
                                    json.ActiveTestList[i].Link,
                                    json.ActiveTestList[i].StartedBy,
                                    json.ActiveTestList[i].StartDate,
                                    json.ActiveTestList[i].EndDate,
                                    json.ActiveTestList[i].ParticipationPercentage,
                                    json.ActiveTestList[i].Views,
                                    json.ActiveTestList[i].Conversions);
                            }

                            // Update variables
                            window.globalPageVar = json.Page;
                            window.globalResultsVar = json.ActiveTestList.length;

                            that.abtestpagecounter.innerHTML = "Page " + (window.globalPageVar + 1);

                            // Update the buttons
                            that._updatePagingButtonsState();

                            // Update the drop down
                            that._updateDropDownSiteList(that.abtestpagesiteselector, json.Sites);

                            // Update the view table Html
                            that.abtesttable = tbl;
                        }
                    });
            },

            // Works with the table
            _addCell(tr, val) {
                var td = document.createElement('td');
                td.innerHTML = val;
                tr.appendChild(td);
            },

            _addRow(tbl, title, link, startedBy, startDate, endDate, participation, views, conversions) {
                var tr = document.createElement('tr');

                if (link === null) {
                    this._addCell(tr, title);
                } else {
                    this._addCell(tr, '<a href=\"' + link + '\" target=\"_blank\">' + title + '</a>');
                }

                this._addCell(tr, startedBy);
                this._addCell(tr, startDate);
                this._addCell(tr, endDate);
                this._addCell(tr, participation);
                this._addCell(tr, views);
                this._addCell(tr, conversions);
                tbl.appendChild(tr);
            },

            // Paging
            _updatePagingButtonsState() {
                // Next
                if (window.globalResultsVar === 5) {
                    this.abtestpagenext.classList.remove("hidden");
                } else {
                    this.abtestpagenext.classList.add("hidden");
                }

                // Prev
                if (window.globalPageVar > 0) {
                    this.abtestpageprevious.classList.remove("hidden");
                } else {
                    this.abtestpageprevious.classList.add("hidden");
                }
            },

            _setupButtonClickListeners() {
                var that = this;

                this.abtestpagenext.onclick = function () {
                    window.globalPageVar = window.globalPageVar + 1;

                    that._getAbTests();
                };

                this.abtestpageprevious.onclick = function () {
                    window.globalPageVar = window.globalPageVar - 1;
                    if (window.globalPageVar < 0) {
                        window.globalPageVar = 0;
                    }

                    that._getAbTests();
                };
            },

            // Drop down
            _updateDropDownSiteList(element, sitesArr) {
                var that = this;
                if (window.globalDropDownPopulated)
                    return true;

                if (sitesArr.length > 2) {
                    element.classList.remove("hidden");
                }

                for (var i = 0; i < sitesArr.length; i++) {
                    var opt = document.createElement('option');

                    opt.appendChild(document.createTextNode(sitesArr[i]));
                    opt.value = sitesArr[i];

                    element.appendChild(opt);
                }

                // Setup the event listener to update our filter flag
                element.onchange = function () {
                    window.globalDropDownFilterSite = element.options[element.selectedIndex].value;
                    window.globalPageVar = 0;
                    window.globalResultsVar = 0;

                    that._getAbTests();
                };

                // We don't need to keep doing this
                window.globalDropDownPopulated = true;
            }
        });
    });