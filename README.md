# Product recommendations built using Azure Machine Learning
Product recommendations built using Azure Machine Learning. Automatically recommend items on your product page. Provide recommendations unique to customer and personalize their experience.

## Module installation
Install the module:
* Automatically: in VC Manager go to Configuration -> Modules -> Product recommendations module (preview) -> Install
* Manually: download module zip package from https://github.com/VirtoCommerce/vc-module-product-recommendations/releases. In VC Manager go to Configuration -> Modules -> Advanced -> Upload module package -> Install.

## Storefront installation and configuration
1. Snippet configuration
* Open template file where you want to display recommended products (ex. product.liquid)
* Add snipped call with the following code:
```html
{% include 'recommendations', provider: 'Cognitive', type: 'Recommendations', product_ids: product.id, size: 5 %}
```
* * `provider` key can correspond to the following values:
* * * `Cognitive` - provide a products from the Azure Machine Learning Service
* * * `Association` - provide a products from accosiations lists, configured in Catalog module
* * for the `product_ids` key we should set the current product id
* * `size` key indicates the lenght of the result products list

2. User events gathering configuration

The products recommendations are based on the history of items that were of interest to the user. To improve the result set of the products, you should enable option for background collection of user events statistics. To do this open settings file `settings_data.json` and provide new setting entry:
```json
"recommendations_enable": true
```

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

## Personalized recommendations
Use the purchase history of a particular customer to provide recommendations unique to that customer and personalize their experience.

## Increase discoverability
Learn from click patterns to increase your product catalog’s discoverability and boost sales.

## Frequently bought together
Automatically recommend items on your product page that are likely to be consumed together in the same transaction.
