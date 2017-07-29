<!DOCTYPE html>
<html>
  <head>
    <style>
      table {
      font-family: arial, sans-serif;
      border-collapse: collapse;
      width: 100%;
      margin: 10px;
      }

      td, th {
      border: 1px solid #dddddd;
      text-align: left;
      padding: 8px;
      }

      tr:nth-child(even) {
      background-color: #eee;
      }

      .tableCategories {

      }

      .wrapper {
      width:100%;
      }

      .content {
      margin: 0 auto;
      width:50%;
      padding: 10px;
      }

    </style>
  </head>
  <body>

    <div class="wrapper">
      <div class="content">

        <?php

/*
*
* PHP Script for Wordpress Migration
* Add your login details below and upload to your server and execute via url in your browser
* Will display feed of the CRUD operations
*
*/

ini_set('error_reporting', E_ERROR);
register_shutdown_function("fatal_handler");
function fatal_handler() {
$error = error_get_last();
print_r($error);
}

//export xml path
$xmlToProcess = "data/xml/export.xml";
$xmlToProcessContent = file_get_contents($xmlToProcess);

//gallery xml path
$galleryToProcess = "data/xml/galleries.xml";
$galleryToProcessContent = file_get_contents($galleryToProcess);

//wordpress db connection


//foreach on xml > export > categories
try {

	//html table
	echo '<table class="tableCategories">
	  <tr>
	    <th>Category Name</th>
	    <th>Category ID</th>
	    <th>Sync Timestamp</th>
	  </tr>';

	$categories = simplexml_load_file($xmlToProcess);
	foreach($categories->categories as $category) {
	   
	   
	   foreach($category->category as $categoryItem) {
	   		
	   		echo '<tr>';
	   		echo '<td>'.$categoryItem->category_name.'</td>';
	   		echo '<td>'.$categoryItem->category_id.'</td>';
	   		echo '<td>--</td>';
	   		
	   		echo '</tr>';
	   }
	   
	}
	
	echo '</table>';
	
	//html table
	echo '<table class="tableForums">
	  <tr>
	    <th>Forum Name</th>
	    <th>Forum ID</th>
	    <th>Category ID</th>
	    <th>Sync Timestamp</th>
	  </tr>';

	$forums = simplexml_load_file($xmlToProcess);
	foreach($forums->forums as $forum) {
	   
	   
	   foreach($forum->forum as $forumItem) {
	   		
	   		echo '<tr>';
	   		echo '<td>'.$forumItem->forum_name.'</td>';
	   		echo '<td>'.$forumItem->forum_id.'</td>';
	   		echo '<td>'.$forumItem->category_id.'</td>';
	   		echo '<td>--</td>';
	   		
	   		echo '</tr>';
	   }
	   
	}
	
	echo '</table>';
	
	//html table
	echo '<table class="threadsForums">
	  <tr>
	    <th>Thread Name</th>
	    <th>Thread ID</th>
	    <th>Forum ID</th>
	    <th>Category ID</th>
	    <th>Sync Timestamp</th>
	  </tr>';

	$threads = simplexml_load_file($xmlToProcess);
	foreach($threads->threads as $thread) {
	   
	   
	   foreach($thread->thread as $threadItem) {
	   		
	   		echo '<tr>';
	   		echo '<td>'.$threadItem->thread_name.'</td>';
	   		echo '<td>'.$threadItem->thread_id.'</td>';
	   		echo '<td>'.$threadItem->forum_id.'</td>';
	   		echo '<td>'.$threadItem->category_id.'</td>';
	   		echo '<td>--</td>';	   		
	   		echo '</tr>';
	   }
	   
	}
	
	echo '</table>';
	
	//html table
	echo '<table class="usersForums">
	  <tr>
	    <th>User Name</th>
	    <th>User ID</th>
	    <th>Sync Timestamp</th>
	  </tr>';

	$users = simplexml_load_file($xmlToProcess);
	foreach($users->users as $user) {
	   
	   
	   foreach($user->user as $userItem) {
	   		
	   		echo '<tr>';
	   		echo '<td>'.$userItem->user_name.'</td>';
	   		echo '<td>'.$userItem->user_id.'</td>';
	   		echo '<td>--</td>';	   		
	   		echo '</tr>';	   			   			  
	   		
	   		
	   		/*
	   		
	   		echo '<table class="usersForums">
			  <tr>
			    <th>Thread Name</th>
			    <th>Content</th>
			   
			  </tr>';
	  
	   		foreach($userItem->posts as $userPosts) {
	   			foreach($userPosts->post as $userPostItem) {
	   				echo '<tr>';
	   				echo '<td>'.$userPostItem->thread_subject.'</td>';
	   				echo '<td>'.$userPostItem->post_content.'</td>';
	   				echo '</tr>';
	   		}
	   		}
	   		
	   		echo '</table>';
	   		
	   		
	   		*/	   		
	   }
	   
	}
	
	echo '</table>';
  
  //html table
	echo '<table class="tableGalleryCategories">
	  <tr>
	    <th>Gallery Name</th>
	    <th>Gallery ID</th>
	    <th>Sync Timestamp</th>
	  </tr>';

	$categoriesGallery = simplexml_load_file($galleryToProcess);
	foreach($categoriesGallery->categories as $category) {
	   
	   
	   foreach($category->category as $categoryItem) {
	   		
	   		echo '<tr>';
	   		echo '<td>'.$categoryItem->name.'</td>';
	   		echo '<td>'.$categoryItem->id.'</td>';
	   		echo '<td>--</td>';
	   		
	   		echo '</tr>';
	   }
	   
	}
	
	echo '</table>';
  
  

}catch(Exception $error) {


}

//foreach on xml > export > forums


//foreach on xml > export > threads


//foreach on xml > export > users > user > post




?>
      </div>
    </div>

  </body>
</html>