# Product recommendations built using Azure Machine Learning
Product recommendations built using Azure Machine Learning. Automatically recommend items on your product page. Provide recommendations unique to customer and personalize their experience.

## Module installation
Install the module:
* Automatically: in VC Manager go to Configuration -> Modules -> Product recommendations module (preview) -> Install
* Manually: download module zip package from https://github.com/VirtoCommerce/vc-module-product-recommendations/releases. In VC Manager go to Configuration -> Modules -> Advanced -> Upload module package -> Install.

## Module configuration
1. Collect data:
* Open Store where you plan to use recommendations
 * Open Recommendations widget
 * * Export catalog
 * * Export usage data
 * * * If you doesn't have usage data at this moment, wait until it will be collected or prepare such file from other sources

![Export](https://cloud.githubusercontent.com/assets/6369252/24508625/76309dd6-157d-11e7-91a4-e7e57e53eff6.png)

2. Create *Cognitive Services APIs account* for *Recommendations API* at [Azure portal](https://portal.azure.com/) in your Azure subscription and obtain a `Key`


![Cognitive Services Account creation](https://cloud.githubusercontent.com/assets/6369252/24510020/6b4eb494-1581-11e7-9a39-d7bd2ab290cd.png)
![Recommendations API Key](https://cloud.githubusercontent.com/assets/6369252/24510073/8d7f40ba-1581-11e7-8321-3fc20c4a0afa.png)

3. Go to [Recommendations UI](https://recommendations-portal.azurewebsites.net)
* Log in to the portal by entering your `Key` in the `Account Key` field
![Log in to Recommendations UI](https://docs.microsoft.com/en-us/azure/media/cognitive-services/reco_signin.png)
* Create a project
![Project creation](https://docs.microsoft.com/en-us/azure/media/cognitive-services/reco_projects.png)
* Upload the catalog you got in step `#1`
* Upload the usage data you got in step `#1`
* Create a build. Select `Build Type` as `Recommendations`
![Build](https://docs.microsoft.com/en-us/azure/media/cognitive-services/reco_firstmodel.png)
4. Configure settings
 * Open store settings. The following settings required:
 * * `Recommendations API Key`. Copy your `Key` you got in step `#2`
 * * `Model ID`. Copy `Model ID` from your project in *Recommendations UI*
 * * `Build ID`. copy `Build ID` from a build in your project in *Recommendations UI*
 
![Settings](https://cloud.githubusercontent.com/assets/6369252/24510451/b2e97a90-1582-11e7-91d4-1981dabab136.png)

# Supported types
## Personalized recommendations
Based on users activity (collected usage data) Cognitive Service will form top of iterested products. User will see this products as recommended, excluding products that he/she already seen. If there is no usage history for the user, he/she will see nothing.

# Planned
## Product recommendations
This recommendations will allow your customer to discover products that are likely to be visited by people that have visited the source item.

## Frequently bought together
Automatically recommend items on your product page that are likely to be consumed together in the same transaction.


## Storefront installation and configuration
1. Snippet configuration
* Open template file where you want to display recommended products (ex. product.liquid)
* Add snipped call with the following code:
```html
{% include 'recommendations', provider: 'Cognitive', type: 'User2Item', product_ids: product.id, take: 5 %}
```
* * `provider` key can correspond to the following values:
* * * `Cognitive` - provide a products from the Azure Machine Learning Service
* * * `Association` - provide a products from associations lists, configured in Catalog module
* * for the `product_ids` key we should set the current product id, or comma-separated identifiers in other cases. This is an optional parameter
* * `take` key indicates the lenght of the result products list we want to receive

2. User events gathering configuration for standard theme

The products recommendations are based on the history of items that were of interest to the user. To improve the result set of the products, you should enable option for background collection of user events statistics. To do this just open settings file `settings_data.json` and provide new setting entry for desired theme:
```json
"collect_user_events_enabled": true
```

3. User events gathering configuration for custom themes

There will be a little more configuration steps for custom theme.
* Provide `collect_user_events_enabled` setting entry in settings file of your theme as discribed in previous step.
* Reference `interactor.js` library (already included in default theme script bundle).
* Include next script block in the footer template.
```js
window.startRecordInteraction = function()
{
    {% if settings.collect_user_events_enabled %}
    var interactions = new Interactor({
        interactions            : true,
        interactionElements     : ["Click", "AddShopCart", "RemoveShopCart", "RecommendationClick"],
        interactionEvents       : ["mouseup", "touchend"],
        endpoint                : "{{ '/storefrontapi/useractions/eventinfo' | absolute_url }}",
        async                   : true,
        debug                   : false
    });
    {% endif %}
}
window.startRecordInteraction();
```

* * There are few types of user events we can collect: `Click`, `AddShopCart`, `RemoveShopCart`, `RecommendationClick`

* To track a users interactions with an element, simply add the event keyword in CSS attribute of the element. Next block of code show how to collect `Click` events for the products:
```html
<a href="{{ product.url | absolute_url }}" class="product-grid-item Click" interactor-arg="{{ product.id }}">
...
```
* * `Click` keyword should be placed in `class` attribute.
* * Custom attribute `interactor-arg` contains product id that will be pass to the endpoint with other information.

All user events will be collect locally and pass to the endpoint before user leaves the page.

