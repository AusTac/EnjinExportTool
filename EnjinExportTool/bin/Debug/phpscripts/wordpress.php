<!DOCTYPE html>
<html>
  <head>
  
  	  <title>Enjin Export Tool - Report</title>
	  <meta charset="utf-8">
	  <meta name="viewport" content="width=device-width, initial-scale=1">
	  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
	  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
	  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
  
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
  <?php 
  
  $param = $_GET["view"];
  
  ?>
  
  
  <body>

    <div class="wrapper">
    <div class="container" style="margin-top:50px">
      
      <nav class="navbar navbar-inverse navbar-fixed-top">
	  <div class="container">
	    <div class="navbar-header">
	      <a class="navbar-brand" href="index.php?view=">Enjin Export Tool</a>
	    </div>
	    <ul class="nav navbar-nav">
	      <li class="<?php if($param == '' || $param == 'users'){ echo 'active';};?> " ><a href="index.php?view=users">Users</a></li>
	      <li class="<?php if($param == 'forum_categories'){ echo 'active';};?> "><a href="index.php?view=forum_categories">Forum Caterories</a></li>
	      <li class="<?php if($param == 'forum_threads'){ echo 'active';};?> "><a href="index.php?view=forum_threads">Forum Threads</a></li>
	      <li class="<?php if($param == 'gallery'){ echo 'active';};?> "><a href="index.php?view=gallery">Gallery</a></li>
	    </ul>
	  </div>
	</nav>

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

	if($param == 'forum_categories'){
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
	
	}
	
	if($param == 'forum_threads'){
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
	
	}
	
	
	if($param == '' || $param == 'users'){
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
	
	}
  
  	if($param == 'gallery'){
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
	
		echo '<table class="tableGalleryCategories">
		  <tr>
		    <th>Image</th>
		    <th>Image Title</th>
		    <th>Gallery Catergory</th>
		    <th>Sync Name</th>
		  </tr>';
            
           foreach($categoriesGallery->images as $image) {
	   
	   
			   foreach($image->image as $imageItem) {
			   	$catName = '';
			   	
			   	foreach($categoriesGallery->categories as $category) {
						foreach($category->category as $categoryItem) {
					   		
					   		if($imageItem->album_id == $categoryItem->id) {
								
								$catName = $categoryItem->name;
								
							}
					   }
					   	
	              }
	              
	                echo '<tr>';
			   		echo '<td><img style="width: 100px;" src="'.$imageItem->thumbImagePath.'"></td>';
			   		echo '<td>'.$imageItem->title.'</td>';
			   		echo '<td>'.$catName.' ('.$imageItem->album_id.')</td>';
			   		echo '<td>--</td>';
			   		
			   		echo '</tr>';
			   
			   }
	   
		} 
            
        echo '</table>';
  
  }

}catch(Exception $error) {


}

?>
      </div>
    </div>
  </body>
</html>