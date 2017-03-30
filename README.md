# Product recommendations built using Azure Machine Learning
Product recommendations built using Azure Machine Learning. Automatically recommend items on your product page. Provide recommendations unique to customer and personalize their experience.

## Installation
1. Create *Cognitive Services APIs account* in you Azure subscription. Select `API type` as `Recommendations API`
2. Go to [Recommendations API UI](https://recommendations-portal.azurewebsites.net)
* Sign in using your `Key 1` or `Key 2` from `Keys` in your *Cognitive Services APIs account*
* Add new project. Project name doesn't matter
2. Install the module:
* Automatically: in VC Manager go to Configuration → Modules → Product recommendations module → Install
 * Manually, for release version: download module zip package from https://github.com/VirtoCommerce/vc-module-product-recommendations/releases. In VC Manager go to Configuration → Modules → Advanced → Upload module package → Install.
 * Manually, for dev version:
 * * Clone https://github.com/VirtoCommerce/vc-module-product-recommendations and checkout dev branch
 * * Open solution in Visual Studio
 * * Open Package Manager Console
 * * Select VirtoCommerce.ProductRecommendationsModule.Web as Default project
 * * Run Compress-Module command
 * * In VC Manager go to Configuration → Modules → Advanced → Upload module package → Install
3. Configure settings
 * Open Store where you plan to use recommendations 
 * Open Recommendations widget in Store
 * * Export catalog
 * * Export usage data
 * * Upload catalog and usage data to your project in *Recommendations API UI*
 * * Create a build. Select `Build Type` as `Recommendations`
 * In Store, open Settings. The following settings required:
 * * `Recommendations API Key`. Copy your `Key 1` or `Key 2` from *Cognitive Services APIs account*
 * * `Model ID`. Copy `Model ID` from your project in *Recommendations API UI*
 * * `Build ID`. copy `Build ID` from a build in your project in *Recommendations API UI*
 * In addition, you may configure the following settings in Store → Settings:
 * * `Recommendations API URL`. Base URL for *Cognitive Services Recommendations API*
 * * `Chunk size of catalog upload file`. Maximum file size of catalog upload file (in MB). If file will exceed this size, it will be separated to multiple files with size no more than specified in this setting. *Cognitive Services Recommendations API* has a limit to maximum size of catalog upload file. Look at documentation in Cognitive Services Recommendations API portal for more info.
 * * `Chunk size of usage events upload file`. Same as `Chunk size of catalog upload file`, but for usage events.
 * * `Maximum number of usage events`. *Cognitive Services Recommendations API* has a limit to maximum count of events that can be uploaded. Look at documentation in Cognitive Services Recommendations API portal for more info.

## Personalized recommendations
Use the purchase history of a particular customer to provide recommendations unique to that customer and personalize their experience.

## Increase discoverability
Learn from click patterns to increase your product catalog’s discoverability and boost sales.

## Frequently bought together
Automatically recommend items on your product page that are likely to be consumed together in the same transaction.
