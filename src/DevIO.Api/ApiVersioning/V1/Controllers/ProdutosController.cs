using AutoMapper;
using DevIO.Api.Controllers;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.ApiVersioning.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository produtoRepository;
        private readonly IProdutoService produtoService;
        private readonly IMapper mapper;

        public ProdutosController(IProdutoRepository produtoRepository,
            IProdutoService produtoService,
            IMapper mapper,
            INotificador notificador) : base(notificador)
        {
            this.produtoRepository = produtoRepository;
            this.produtoService = produtoService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoViewModel>>> ObterTodos()
        {
            var produtosViewModel = mapper.Map<IEnumerable<ProdutoViewModel>>(await produtoRepository.ObterProdutosFornecedores());

            return CustomResponse(produtosViewModel);
        }

        [HttpGet("id:guid")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var produtoViewModel = mapper.Map<ProdutoViewModel>(await produtoRepository.ObterPorId(id));

            if (produtoViewModel is null) return NotFound();

            return CustomResponse(produtoViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            var imagemNome = $"{produtoViewModel.Imagem}_{Guid.NewGuid()}";

            var uploadArquivoSucesso =
                UploadArquivo(produtoViewModel.ImagemUpload, imagemNome);

            if (uploadArquivoSucesso)
                await produtoService.Adicionar(mapper.Map<Produto>(produtoViewModel));

            produtoViewModel.Imagem = imagemNome;
            return CustomResponse(produtoViewModel);
        }


        [HttpPost]
        [Route("adicionar-formfile")]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAlternativo(ProdutoImagemViewModel produtoImagemViewModel)
        {
            var imgPrefix = $"{Guid.NewGuid()}_";

            var uploadArquivoSucesso =
                await UploadArquivoAlternativo(produtoImagemViewModel.ImagemUpload, imgPrefix);

            if (uploadArquivoSucesso)
                await produtoService.Adicionar(mapper.Map<Produto>(produtoImagemViewModel));

            produtoImagemViewModel.Imagem = imgPrefix + produtoImagemViewModel.ImagemUpload.FileName;
            return CustomResponse(produtoImagemViewModel);
        }


        [RequestSizeLimit(40000000)]
        [HttpPost("imagem")]
        public async Task<ActionResult> AdicionarImagem(IFormFile file)
        {
            return Ok();
        }


        [HttpDelete("id:guid")]
        public async Task<ActionResult<IEnumerable<ProdutoViewModel>>> Remover(Guid id)
        {
            var produtoViewModel = mapper.Map<ProdutoViewModel>(await produtoRepository.ObterPorId(id));

            if (produtoViewModel is null) return NotFound();

            await produtoRepository.Remover(id);

            return Ok(produtoViewModel);
        }


        #region Metodos Privados
        private bool UploadArquivo(string arquivo64, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo64) || string.IsNullOrEmpty(imgNome))
            {
                NotificarErro("Forneça uma imagem e o nome dela para esse produto!");
                return false;
            }

            var imagemDataByteArray = Convert.FromBase64String(arquivo64);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com esse nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imagemDataByteArray);
            return true;

        }

        private async Task<bool> UploadArquivoAlternativo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para este produtoo!");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com esse nome!");
                return false;
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return true;

        }
        #endregion

    }
}

