# README #

This README would normally document whatever steps are necessary to get your application up and running.

## JIRA ##
Dashboard là màn hình hiển thị tổng quan về project. Như còn lai  bao nhiêu ngày.
Task nào đang làm, task của mình là gì...

1. Login bằng account đã gửi
1. [Dashboard](https://onepiece.atlassian.net/secure/ConfigurePortalPages!default.jspa)  -> manager Dashboard  -> Popular  -> click vào onepiece  -> add to favorites

### JIRA Issue ###

1. Check những task mình phải làm. Tiến độ [here](https://onepiece.atlassian.net/secure/RapidBoard.jspa?rapidView=1&quickFilter=2)

2. [Planning](https://onepiece.atlassian.net/secure/RapidBoard.jspa?rapidView=1&view=planning)


### JIRA plugin ###
* [Admin](https://onepiece.atlassian.net/plugins/)


## OnePiece Code Convention ##

* Làm thêm màn hình mới
  * Thiết kế UI = NGUI rồi lưu ở dạng prefab trong thư mục Resources/Views
  * Xử lý UI = cách dùng class UI chung ở lớp Core
  * Với mỗi màn hình phải viết script xử lý kế thừa lại lớp View chung
* Ví dụ : màn hình start loading
  * thiết kế view rồi lưu prefab Start ở thư mục Resources/Views
  * xử lý event click button Start ở màn hình start loading
    * xem script StartView.cs kế thừa lại lớp base View
    * dùng hàm AttachButton(GameObject, EventDelegate.Callback) của lớp UI
    * load thêm view trong hàm onStartBtnClicked() dùng lớp ViewLoader
* các sự kiện trong màn hình view có thể override 
  * OnOpen
  * OnClose
  * OnSuspend
  * OnResume
* hạn chế tạo thêm scene, mỗi màn hình chỉ nên tương ứng 1 prefab 

## OnePiece Project Code Detail ##

Project structure

* Audio : 
  * BGM : âm thanh background
  * SE: âm thanh khi ấn button, các hành động hit, attack..
* MsgPack: plugin load data sử dụng message pack
* NGUI : plugin để thiết kế UI
* Resources: resource của game (ban đầu là của )
  * data: ảnh monster, character,….
  * Prefab: chứa game object đc định nghĩa trước 
    * Blocks: đối tượng nút trên màn hình
    * Monster : quái ( thiết kế dùng Spine plugin)
    * particle : hiệu ứng particle
    * UI : UI
  * Resource :ảnh, animation, font,….
    * Animation : animation của monster, character
    * Font: font sử dụng trong UI
    * Spine: chứa data của monster , material , skeleton...
    * Texture : texture (load ảnh để vào đây)
  * Spine: plugin để thiết kế character, monster
    * spine-csharp: code
    * spine-unity: resource
  * Views: chứa các prefab định nghĩa view của game
    * hiện mới có Title prefab định nghĩa màn hình ban đầu lúc vào game
    * mỗi màn hình tương ứng với 1 view, sau sẽ thiết kế thêm LoginView, GameView...
* Scenes: scene của game (sử dụng để load trong Unity)
  * common_scene: scene chứa common_object trong đó object này dùng để quản lý DB (nếu có), media (sound), xử lý các coroutine nói chung...
  * main_scene: scene chính của game
  * 3typePuzzle: các scene sử dụng trong plugin 3 type puzzle (hiện đang dùng để tham khảo, sau sẽ xử lý load object bằng cách khác: thiết kế prefab để ở thư mục Resources/View)
* Scripts: code của game
  * App : code xử lý sau này tương ứng với mỗi màn hình thì tạo thêm 1 thư mục ( tạm thời chưa có) 
  	* Views : script xử lý cho từng màn hình 
  * Core : framework chính của game (tạm thời tham khảo UnityBase)
    * 3typePuzzle: code của plugin 3typePuzzle
    * Animation → Game Animation
    * API → load data from server 
    * Attribute  → 
    * DAO → access DB
    * Debug → debug script
    * Factory → singleton 
    * Manager → manage DB, event, file, sound, file…
    * Media → audio, movie..
    * ObjectPoolManager → 
    * Scenes → load scene
    * Serializable → 
    * Tool → 
    * Utility → utility class
    * Views → handle ui
  * Env: định nghĩa các biển tổng quát, tạm thời để ở file Config
  * Test: viết test code
* SimpleSQL : dùng để load db
* sqlitekit: plugin load db
* Standard Assets: các asset mặc định của Unity
* UnityTestTools : test tool của Unity

## Viết test code ##

* Phải viết test code cho.
  * Model
  * Service
* Để file test vào floder Assets/Script/App/Test/Editor
  * Chú ý bắt buộc phải đẻ vào Editor k thì sẽ k chạy
  * Tham khảo thêm ở [đây](http://tipsnote.tumblr.com/post/101751195305/unity-memo)
