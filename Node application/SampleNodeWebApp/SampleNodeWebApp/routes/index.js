
/*
 * GET home page.
 */

exports.index = function(req, res){
  res.render('index', { title: 'Express' });
};


exports.indexhtml = function(req, res){
  res.render('Gruntfileconverstiontask', { title: 'This is a sample page' });
};

exports.indexGhtml = function(req, res){
  res.render('Gruntfileconverstiontask', { title: 'This is a sample page1' });
};